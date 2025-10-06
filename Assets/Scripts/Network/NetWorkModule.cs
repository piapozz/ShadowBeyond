using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using static GameEnum;

public class NetWorkModule
{
    public struct SendData
    {
        public SyncType type;
        public int[] param;
    }

    /// <summary>
    /// 同期内容を送信
    /// </summary>
    /// <param name="type"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static async UniTask SendSyncData(SendData sendData)
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        byte[] byteData = {};
        // 送信データを作成
        switch (sendData.type)
        {
            case SyncType.DECK_DATA:
                break;
            case SyncType.SEED:
                break;
            case SyncType.INPUT:
                byteData = PackInputData(sendData.param);
                break;
            default: break;
        }

        PhotonNetwork.RaiseEvent((byte)sendData.type, byteData, options, sendOptions);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 入力データをパッキングする
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private static byte[] PackInputData(int[] param)
    {
        ulong pack = 0;
        pack |= (ulong)param[0] & 0xF;

        for (int i = 1, max = param.Length; i < max; i++)
        {
            pack |= ((ulong)param[i] & 0xF) << (4 * (i + 1));
        }

        byte[] bytes = new byte[5];
        for (int i = 0; i < 5; i++)
            bytes[i] = (byte)(pack >> (8 * i));

        return bytes;
    }

    private static int[] UnpackInputData(byte[] packData, ref InputType input, ref int[] param)
    {
        // byteからulongに変換
        ulong unPack = 0;
        int paramSize = packData.Length;
        for (int i = 0; i < paramSize; i++)
        {
            unPack |= (ulong)packData[i] << (8 * i);
        }

        // 取り出し
        InputType type = (InputType)((unPack >> 4) & 0xF);
        for (int i = 1; i < (paramSize - 1); i++)
        {
            param[i] = (int)((unPack >> (4 * i)) & 0xF);
        }

        return param;
    }
}
