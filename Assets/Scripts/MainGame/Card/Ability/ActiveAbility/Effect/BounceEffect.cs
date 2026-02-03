using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : BaseEffect
{
    public BounceEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        bool isOwn = targetCard.GetObject().isLocal;
        int playerIndex = isOwn ? 0 : 1;
        Hand targetHand = BattleManager.instance.GetPlayer(playerIndex).hand;
        // フィールドから除外
        BattleManager.instance.field.RemoveCard(targetCard);
        // 手札に戻す
        targetHand.AddCard(targetCard);
        UIManager.instance.AddHandCard(playerIndex, new List<CardData> { targetCard });
        // バウンスアニメーション
        List<CardObject> bounceCards = new List<CardObject> { targetCard.GetObject() };
        UIManager.instance.SetBounceSequence(bounceCards, isOwn);
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        bool isOwn = targetCards[0].GetObject().isLocal;
        int playerIndex = isOwn ? 0 : 1;
        Hand targetHand = BattleManager.instance.GetPlayer(playerIndex).hand;
        List<CardObject> bounceCards = new List<CardObject>();
        for (int i = 0, max = targetCards.Count; i < max; i++)
        {
            // フィールドから除外
            BattleManager.instance.field.RemoveCard(targetCards[i]);
            // 手札に戻す
            targetHand.AddCard(targetCards[i]);
            UIManager.instance.AddHandCard(playerIndex, new List<CardData> { targetCards[i] });
            bounceCards.Add(targetCards[i].GetObject());
        }
        // バウンスアニメーション
        UIManager.instance.SetBounceSequence(bounceCards, isOwn);
    }
}
