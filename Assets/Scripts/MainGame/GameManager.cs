using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameEnum;

public class GameManager : SystemObject
{
    public struct SendData
    {
        public SyncType type;
        public int[] param;
    }

    private int _seed = -1;
    private Queue<SendData> _recieveQueue = null;
    private TurnManager _turnManager = null;

    public async override UniTask Initialize()
    {
        _recieveQueue = new Queue<SendData>();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        _turnManager = new TurnManager();
        await StartBattle();
    }

    private void OnEvent(EventData photonEvent)
    {
        byte[] data = (byte[])photonEvent.CustomData;
    }

    /// <summary>
    /// 試合開始
    /// </summary>
    /// <returns></returns>
    private async UniTask StartBattle()
    {
        // オーナーがシード値を決定し同期
        if (PhotonNetwork.IsMasterClient)
        {
            _seed = Environment.TickCount;
            SendData sendData = new SendData();
            sendData.type = SyncType.SEED;
            sendData.param = new int[] { _seed };
            await NetWorkModule.SendSyncData(sendData);
        }

        await TurnProc();
    }

    /// <summary>
    /// 各ターン中の処理
    /// </summary>
    /// <returns></returns>
    private async UniTask TurnProc()
    {
        // 非同期で処理を開始
        var task = StartInput();

        // ターン終了まで
        while (true)
        {
            await UniTask.DelayFrame(1);

            // 入力された種類を取得
            InputType type = await task;
            int[] param = new int[] { };
            // 入力されていたら送信
            if (type == InputType.INVALID) continue;
            SendData sendData = new SendData();
            sendData.type = SyncType.INPUT;
            sendData.param = param;
            await NetWorkModule.SendSyncData(sendData);

            // 入力に応じた処理
            bool isTurnEnd = await ExcuteEvent(type, param);
            if (isTurnEnd) break;
        }
    }

    // UIManagerに実装
    private async UniTask<InputType> StartInput()
    {
        await UniTask.CompletedTask;
        return InputType.INVALID;
    }

    private async UniTask RecieveData()
    {

    }

    /// <summary>
    /// 入力に応じた処理
    /// </summary>
    /// <param name="type"></param>
    /// <param name="param"></param>
    /// <returns>ターン終了か否か</returns>
    private async UniTask<bool> ExcuteEvent(InputType type, int[] param = null)
    {
        bool isTurnEnd = false;
        switch (type)
        {
            case InputType.PLAY_CARD:
                // ファンファーレ発動
                break;
            case InputType.ATTACK:
                // 攻撃時能力
                // 交戦時能力
                // ダメージ処理
                break;
            case InputType.EVOLVE:
                // 進化時能力
                break;
            case InputType.SUPER_EVOLVE:
                // 超進化時能力
                break;
            case InputType.ACT:
                // アクト時能力
                break;
            case InputType.FUSION:
                // 融合時能力
                break;
            case InputType.TURN_END:
                isTurnEnd = true;
                break;
            default: break;
        }

        EffectManager.instance.ExecuteEffect();

        return isTurnEnd;
    }

    /// <summary>
    /// 試合終了
    /// </summary>
    /// <returns></returns>
    private async UniTask EndBattle()
    {
        await UniTask.CompletedTask;
    }
}
