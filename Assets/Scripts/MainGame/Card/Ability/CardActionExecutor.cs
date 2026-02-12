using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionExecutor
{
    //public static async UniTask<bool> TryActAsync(CardData card, bool isOwn)
    //{
    //    var ability = card.ability;

    //    // ターゲット選択が必要ないならスルー
    //    IReadOnlyList<CardObject> targets = null;
    //    if (targetDef != null && targetDef.isSelect)
    //    {
    //        var candidates = TargetSearcher.Search(targetDef, isOwn);

    //        targets = await UIManager.instance.SelectTargetAsync(
    //            candidates,
    //            targetDef.count
    //        );
    //    }

    //    // ここから確定
    //    card.ConsumeAct();
    //    ability.Engage(isOwn, targets);

    //    return true;
    //}
}
