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
    private static async UniTask<TargetSelectResult> SelectTarget(List<BaseComponent> selectTarget, int targetCount)
    {
        // オブジェクトのリストに変換
        List<BaseFieldObject> targetObjects = new List<BaseFieldObject>(targetCount);
        for (int i = 0, max = targetCount; i < max; i++)
        {
            BaseFieldObject obj = selectTarget[i].GetObject();
            targetObjects.Add(obj);
        }
        TargetSelectResult selectResult = await UIManager.instance.SelectTargetAsync(
            targetObjects, targetCount);
        return selectResult;
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
        // ターゲット候補を取得
        List<BaseComponent> targets = BattleManager.instance.GetTargetComponent(selectTarget, isOwn);
        // ターゲット選択が必要ないか対象がないなら普通に実行
        if (selectTarget == null || targets.Count == 0)
        {
            ability.Engage(isOwn);
            return;
        };
        TargetSelectResult selectResult = await SelectTarget(targets, selectTarget.count);
        // 完了していたなら処理を実行
        if (selectResult.result)
        {
            ability.Engage(isOwn, selectResult.selected);
        }
    }
}
