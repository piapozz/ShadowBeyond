using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 【ファンファーレ】【覚醒】なら、自分のリーダーを4回復。
public class CardAbility_001402 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        if (!GetPlayer(isOwn).leader.IsOverflow()) return;
        HealEffect effect = new HealEffect(new List<int> { 4 });
        var targetPlayer = GetPlayer(isOwn);
        BaseComponent component = targetPlayer.leader;
    }
}