using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SystemObject
{
    

    public async override UniTask Initialize()
    {
        await StartBattle();
    }

    /// <summary>
    /// 試合開始
    /// </summary>
    /// <returns></returns>
    private async UniTask StartBattle()
    {
        await TurnProc();
    }

    /// <summary>
    /// 各ターン中の処理
    /// </summary>
    /// <returns></returns>
    private async UniTask TurnProc()
    {
        await EndBattle();
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
