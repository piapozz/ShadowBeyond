using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEnum;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance { get; private set; }

    public int localPlayerId = -1;

    private bool isConnecting = false;

    private NetworkRunner runner;
    private const int MaxBufferCount = 5;
    private const string SessionName = "BattleSession";
    private const int MaxPlayers = 2;


    private readonly Dictionary<int, byte[]> sendHistory = new(); // seq→data
    private readonly SortedDictionary<int, byte[]> recvBuffer = new(); // seq→data

    private int localSequence = 0;     // 自分が送信するデータの通し番号
    private int lastDeliveredSeq = -1; // BattleManagerに最後に渡したデータ番号

    public enum SyncTypeEnum : byte
    {
        SYNC_NONE = 0,
        SYNC_BATTLE = 1,   // バトルデータ（既存）
        SYNC_SEED = 2,     // シード値
        SYNC_RESULT = 3,   // （将来的な拡張用）
    }

    public bool seedReceived { get; private set; } = false;
    private int receivedSeed = 0;
    public int GetReceivedSeed() => receivedSeed;

    public struct SendBattleData
    {
        public InputType type;
        public int[] param;

        // ---- バイト列へ変換 ----
        public byte[] ToBytes()
        {
            ulong pack = 0;
            pack |= (ulong)type & 0xF;

            for (int i = 0, max = param != null ? param.Length : 0; i < max; i++)
            {
                pack |= ((ulong)param[i] & 0xF) << (4 * (i + 1));
            }

            byte[] bytes = new byte[5];
            for (int i = 0; i < 5; i++)
                bytes[i] = (byte)(pack >> (8 * i));

            return bytes;
        }

        // ---- バイト列から復元 ----
        public static SendBattleData FromBytes(byte[] packData)
        {
            SendBattleData data = new SendBattleData();

            ulong unPack = 0;
            for (int i = 0; i < packData.Length; i++)
            {
                unPack |= (ulong)packData[i] << (8 * i);
            }

            // type (下位4bit)
            data.type = (InputType)(unPack & 0xF);

            // param復元（4bit単位）
            int paramCount = (packData.Length * 2) - 1;
            data.param = new int[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                data.param[i] = (int)((unPack >> (4 * (i + 1))) & 0xF);
            }

            return data;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // -----------------------------------
    // 起動
    // -----------------------------------
    public async void StartMatchmaking()
    {
        if (isConnecting) return;
        isConnecting = true;

        Debug.Log("[Network] 🔍 マッチング開始...");

        // NetworkRunner生成
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = false;
        runner.AddCallbacks(this);

        // ゲーム開始（ホストまたはクライアント）
        var result = await runner.StartGame(new StartGameArgs()
    {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = SessionName,
            PlayerCount = MaxPlayers,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (!result.Ok)
        {
            Debug.LogError($"[Network] 接続失敗: {result.ShutdownReason}");
            isConnecting = false;
            return;
        }

        Debug.Log($"[Network] ✅ 接続成功: {result.GetHashCode()}");

        // 👇 プレイヤー人数が揃うまで待機
        StartCoroutine(WaitForPlayers());
    }

    private IEnumerator WaitForPlayers()
    {
        Debug.Log($"[Network] ⏳ プレイヤー待機中... (現在 {runner.SessionInfo.PlayerCount}/{MaxPlayers})");

        // MaxPlayers が揃うまでループ
        while (runner != null && runner.SessionInfo != null && runner.SessionInfo.PlayerCount < MaxPlayers)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (runner == null) yield break;

        if(runner.LocalPlayer != null)
        {
            localPlayerId = runner.LocalPlayer.PlayerId;
            Debug.Log($"[Network] 🎉 自分のPlayerIdは {localPlayerId} です");
        }
        else
        {
            Debug.LogError("[Network] ❌ 自分のPlayerIdが取得できません");
            yield break;
        }

        Debug.Log("[Network] 🎮 全プレイヤーが揃いました！シーンをロードします...");
        SceneManager.LoadScene("MainScene");
    }

    public bool IsConnected()
    {
        return runner != null && isConnecting;
    }

    //==================================
    // コールバック群
    //==================================
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Network] Player joined: {player.PlayerId}");

        // 2人揃ったらバトルシーンへ遷移
        if (runner.ActivePlayers.Count() == MaxPlayers)
        {
            Debug.Log("[Network] 🎮 プレイヤーが揃いました、バトル開始！");
            LoadBattleScene();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Network] Player left: {player.PlayerId}");
    }

    private void LoadBattleScene()
    {
        // バトルシーンをロード（NetworkSceneManager経由）
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            //_runner.SetActiveScene("BattleScene");
        }
    }

    // -----------------------------------
    // データ送信
    // -----------------------------------
    public void SendData(SendBattleData data)
    {
        if (runner == null)
            return;

        byte[] rawData = data.ToBytes();
        if (rawData == null || rawData.Length == 0)
            return;

        int seq = ++localSequence;

        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);

        // 先頭に通信タイプ
        bw.Write((byte)SyncTypeEnum.SYNC_BATTLE);
        bw.Write(seq);
        bw.Write(rawData.Length);
        bw.Write(rawData);

        byte[] fullPacket = ms.ToArray();

        sendHistory[seq] = fullPacket;
        if (sendHistory.Count > MaxBufferCount)
        {
            int oldest = int.MaxValue;
            foreach (int key in sendHistory.Keys)
                oldest = Mathf.Min(oldest, key);
            sendHistory.Remove(oldest);
        }

        foreach (var player in runner.ActivePlayers)
        {
            if (player == runner.LocalPlayer) continue;
            ReliableKey reliable = default;
            runner.SendReliableDataToPlayer(player, reliable, fullPacket);
        }

        Debug.Log($"[Network] Sent [SYNC_BATTLE] seq={seq}, size={rawData.Length}");
    }


    public int SendSeedData(int seedIndex)
    {
        if (runner == null)
            return -1;

        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);

        // 通信タイプを最初に書き込む
        bw.Write((byte)SyncTypeEnum.SYNC_SEED);
        bw.Write(seedIndex);

        byte[] sendData = ms.ToArray();

        if (runner.ActivePlayers.Count() < 2)
        {
            Debug.Log("[Network] ⚠️ まだ相手がいないため送信できません");
            return -1;
        }

        foreach (var player in runner.ActivePlayers)
        {
            if (player == runner.LocalPlayer) continue;
            Debug.Log($"[Network] 🌱 シード値送信: {seedIndex} [size] : {sendData.Length}");

            ReliableKey reliable = default;
            runner.SendReliableDataToPlayer(player, reliable, sendData);
        }

        return 0;
    }

    // -----------------------------------
    // データ受信
    // -----------------------------------
    public void HandleReliableData(byte[] data)
    {
        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader br = new BinaryReader(ms);

        // 1通信タイプを最初に読む
        SyncTypeEnum syncType = (SyncTypeEnum)br.ReadByte();

        switch (syncType)
        {
            // 🌱 シード値受信
            case SyncTypeEnum.SYNC_SEED:
            {
                int seedValue = br.ReadInt32();
                receivedSeed = seedValue;
                seedReceived = true;
                Debug.Log($"[Network] 🌱 シード値を受信: {seedValue}");
                break;
            }

            // ⚔ バトルデータ受信
            case SyncTypeEnum.SYNC_BATTLE:
            {
                int seq = br.ReadInt32();
                int len = br.ReadInt32();
                byte[] payload = br.ReadBytes(len);

                if (recvBuffer.ContainsKey(seq) || seq <= lastDeliveredSeq)
                    return;

                recvBuffer[seq] = payload;

                while (recvBuffer.Count > MaxBufferCount)
                {
                    int oldest = int.MaxValue;
                    foreach (int bufferKey in recvBuffer.Keys)
                        oldest = Mathf.Min(oldest, bufferKey);
                    recvBuffer.Remove(oldest);
                }

                Debug.Log($"[Network] ⚔ 受信 seq={seq}, size={len}");
                break;
            }

            // その他（将来的な拡張）
            default:
                Debug.LogWarning($"[Network] 未対応のSyncType受信: {syncType}");
                break;
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, byte[] data)
    {
        HandleReliableData(data);
    }

    // ArraySegment<byte> 版もこちらに統一
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        HandleReliableData(data.ToArray());
    }


    // -----------------------------------
    // BattleManager が呼び出す
    // 一番古い未処理データを順番に返す
    // -----------------------------------
    public SendBattleData GetNextReceivedData()
    {
        SendBattleData data;
        data.type = InputType.INVALID;
        data.param = null;

        if (Instance == null || runner == null) return data;

        foreach (var kvp in recvBuffer)
        {
            int seq = kvp.Key;
            if (seq > lastDeliveredSeq)
            {
                lastDeliveredSeq = seq;
                recvBuffer.Remove(seq);

                data = SendBattleData.FromBytes(kvp.Value);
                Debug.Log($"[Network] ⚔ 処理データ seq={seq}, type={data.type}, param=[{(data.param != null ? string.Join(",", data.param) : "")}]");

                return data;
            }
        }

        return data;
    }

    public int GetActivePlayerCount()
    {
        return runner.ActivePlayers.Count();
    }

    // -----------------------------------
    // INetworkRunnerCallbacks（空実装OK）
    // -----------------------------------
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
}
