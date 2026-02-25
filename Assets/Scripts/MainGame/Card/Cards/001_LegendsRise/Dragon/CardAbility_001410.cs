using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 自分のデッキから2枚を引く。【覚醒】なら、自分のリーダーを2回復。
public class CardAbility_001410 : BaseCardAbility
{

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        DrawEffect effect = new DrawEffect(new List<int> { 2 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck);
        if (!GetPlayer(isOwn).leader.IsOverflow()) return;
        HealEffect healEffect = new HealEffect(new List<int> { 2 });
        var targetPlayer = GetPlayer(isOwn);
        BaseComponent component = targetPlayer.leader;
    }
}

