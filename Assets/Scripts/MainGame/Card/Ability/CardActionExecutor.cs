using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class CardActionExecutor
{
    /// <summary>
    /// ターゲット選択を開始
    /// </summary>
    /// <param name="selectTarget"></param>
    /// <param name="isOwn"></param>
    /// <returns></returns>
    private static async UniTask<bool> SelectTarget(Target selectTarget, bool isOwn)
    {
        // ターゲット候補を取得
        List<BaseComponent> targets = BattleManager.instance.GetTargetComponent(selectTarget, isOwn);
        // オブジェクトのリストに変換
        List<BaseFieldObject> targetObjects = new List<BaseFieldObject>(targets.Count);
        for (int i = 0, max = targetObjects.Count; i < max; i++)
        {
            BaseFieldObject obj = targets[i].GetObject();
            targetObjects.Add(obj);
        }
        TargetSelectResult selectResult = await UIManager.instance.SelectTargetAsync(
            targetObjects, selectTarget.count);
        return selectResult.result;
    }

    /// <summary>
    /// アクト処理を試行する
    /// </summary>
    /// <param name="card"></param>
    /// <param name="isOwn"></param>
    /// <returns></returns>
    public static async UniTask TryAct(CardData card, bool isOwn)
    {
        Debug.Log("TryAct called");
        var ability = card.ability;
        Target selectTarget = ability.selectTarget[(int)BaseCardAbility.TargetTiming.Engage];
        // ターゲット選択が必要ないならスルー
        if (selectTarget == null) return;

        bool result = await SelectTarget(selectTarget, isOwn);

        // 完了していたなら処理を実行
        if (result)
        {
            ability.Engage(isOwn);
        }
    }
}
