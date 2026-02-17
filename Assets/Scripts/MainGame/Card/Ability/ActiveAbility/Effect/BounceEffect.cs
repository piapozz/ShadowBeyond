using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : BaseEffect
{
    public BounceEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        bool isOwn = context.isOwn;
        Hand targetHand = context.player.hand;
        List<CardData> bounceCards = new List<CardData>();
        List<CardObject> bounceCardObjects = new List<CardObject>();
        foreach (var target in context.targets)
        {
            if (target is CardData targetCard)
            {
                // フィールドから除外
                BattleManager.instance.field.RemoveCard(targetCard, isOwn);
                // 手札に戻す
                targetHand.AddCard(targetCard);
                bounceCards.Add(targetCard);
                bounceCardObjects.Add(targetCard.GetCardObject());
            }
        }
        UIManager.instance.AddHandCard(isOwn, bounceCards);
        // バウンスアニメーション
        UIManager.instance.SetBounceSequence(bounceCardObjects, isOwn);

        return null;
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        bool isOwn = targetCard.GetObject().isLocal;
        int playerIndex = isOwn ? 0 : 1;
        Hand targetHand = BattleManager.instance.GetPlayer(playerIndex).hand;
        // フィールドから除外
        BattleManager.instance.field.RemoveCard(targetCard, isOwn);
        // 手札に戻す
        targetHand.AddCard(targetCard);
        UIManager.instance.AddHandCard(isOwn, new List<CardData> { targetCard });
        // バウンスアニメーション
        List<CardObject> bounceCards = new List<CardObject> { targetCard.GetCardObject() };
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
            BattleManager.instance.field.RemoveCard(targetCards[i], isOwn);
            // 手札に戻す
            targetHand.AddCard(targetCards[i]);
            UIManager.instance.AddHandCard(isOwn, new List<CardData> { targetCards[i] });
            bounceCards.Add(targetCards[i].GetCardObject());
        }
        // バウンスアニメーション
        UIManager.instance.SetBounceSequence(bounceCards, isOwn);
    }
}
