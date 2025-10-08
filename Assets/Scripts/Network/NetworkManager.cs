using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEnum;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance { get; private set; }

    private NetworkRunner _runner;

    private const int MaxBufferCount = 5;
    private bool _isConnecting = false;
    private const string SessionName = "BattleSession";
    private const int MaxPlayers = 2;

    private readonly Dictionary<int, byte[]> sendHistory = new(); // seq→data
    private readonly SortedDictionary<int, byte[]> recvBuffer = new(); // seq→data

    private int localSequence = 0;     // 自分が送信するデータの通し番号
    private int lastDeliveredSeq = -1; // BattleManagerに最後に渡したデータ番号

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
        if (_isConnecting) return;
        _isConnecting = true;

        Debug.Log("[Network] 🔍 マッチング開始...");

        // NetworkRunner生成
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = false;
        _runner.AddCallbacks(this);

        // 既存セッションリスト取得
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = SessionName,
            PlayerCount = MaxPlayers,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (!result.Ok)
        {
            Debug.LogError($"[Network] 接続失敗: {result.ShutdownReason}");
            _isConnecting = false;
            return;
        }

        Debug.Log($"[Network] ✅ 接続成功: {result.GetHashCode()}");
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
        if (_runner == null)
            return;

        byte[] rawData = data.ToBytes();

        if(rawData == null || rawData.Length == 0)
            return;

        // 通し番号をつけて送信データを構築
        int seq = ++localSequence;

        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(seq);
        bw.Write(rawData.Length);
        bw.Write(rawData);

        byte[] fullPacket = ms.ToArray();

        // 履歴に保存（最大3件）
        sendHistory[seq] = fullPacket;
        if (sendHistory.Count > MaxBufferCount)
        {
            int oldest = int.MaxValue;
            foreach (int key in sendHistory.Keys)
                oldest = Mathf.Min(oldest, key);
            sendHistory.Remove(oldest);
        }

        // 全プレイヤーへ信頼性送信
        foreach (var player in _runner.ActivePlayers)
        {
            ReliableKey reliable = default;
            _runner.SendReliableDataToPlayer(player, reliable, fullPacket);
        }

        Debug.Log($"[Network] Sent seq={seq}, size={rawData.Length}");
    }

    // -----------------------------------
    // データ受信
    // -----------------------------------
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, byte[] data)
    {
        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader br = new BinaryReader(ms);

        int seq = br.ReadInt32();
        int len = br.ReadInt32();
        byte[] payload = br.ReadBytes(len);

        // 既に受信済みなら無視（重複排除）
        if (recvBuffer.ContainsKey(seq) || seq <= lastDeliveredSeq)
            return;

        recvBuffer[seq] = payload;

        // 最大3件制限
        while (recvBuffer.Count > MaxBufferCount)
        {
            int oldest = int.MaxValue;
            foreach (int bufferKey in recvBuffer.Keys)
                oldest = Mathf.Min(oldest, bufferKey);
            recvBuffer.Remove(oldest);
        }

        Debug.Log($"[Network] Received seq={seq}, size={len}");
    }

    // -----------------------------------
    // BattleManager が呼び出す
    // 一番古い未処理データを順番に返す
    // -----------------------------------
    public SendBattleData? GetNextReceivedData()
    {
        foreach (var kvp in recvBuffer)
        {
            int seq = kvp.Key;
            if (seq > lastDeliveredSeq)
            {
                lastDeliveredSeq = seq;
                recvBuffer.Remove(seq);
                return SendBattleData.FromBytes(kvp.Value);
            }
        }
        return null;
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

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
}
