using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnDeckEffect : BaseEffect
{
    public ReturnDeckEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        List<CardObject> cardObjects = new List<CardObject>();
        // デッキに追加
        Deck targetDeck = context.player.deck;
        Hand targetHand = context.player.hand;
        for (int i = 0, max = context.targets.Count; i < max; i++)
        {
            // カードでないならスキップ
            if (!(context.targets[i] is CardData targetCard)) continue;
            targetDeck.AddCard(targetCard);
            // 手札から除外
            targetHand.RemoveCard(targetCard);
            cardObjects.Add(targetCard.GetCardObject());
        }
        // 挙動
        UIManager.instance.SetReturnDeckSequence(cardObjects, context.isOwn);
        return null;
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        bool isOwn = targetCard.GetObject().isLocal;
        int playerIndex = isOwn ? 0 : 1;
        // デッキに追加
        Deck targetDeck = BattleManager.instance.GetPlayer(playerIndex).deck;
        targetDeck.AddCard(targetCard);
        // 手札から除外
        Hand targetHand = BattleManager.instance.GetPlayer(playerIndex).hand;
        targetHand.RemoveCard(targetCard);
        // 挙動
        List<CardObject> cardObjects = new List<CardObject>() { targetCard.GetCardObject() };
        UIManager.instance.SetReturnDeckSequence(cardObjects, isOwn);
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        bool isOwn = targetCards[0].GetObject().isLocal;
        int playerIndex = isOwn ? 0 : 1;
        List<CardObject> cardObjects = new List<CardObject>();
        // デッキに追加
        Deck targetDeck = BattleManager.instance.GetPlayer(playerIndex).deck;
        for (int i = 0, max = targetCards.Count; i < max; i++)
        {
            targetDeck.AddCard(targetCards[i]);
            // 手札から除外
            Hand targetHand = BattleManager.instance.GetPlayer(playerIndex).hand;
            targetHand.RemoveCard(targetCards[i]);
            cardObjects.Add(targetCards[i].GetCardObject());
        }
        // 挙動
        UIManager.instance.SetReturnDeckSequence(cardObjects, isOwn);
    }
}
