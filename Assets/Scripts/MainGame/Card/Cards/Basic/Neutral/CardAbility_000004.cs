using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_000004 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Evolve(bool isOwn)
    {
        // 自分のリーダーを2回復
        HealEffect effect = new HealEffect(new List<int> { 2 });
        var targetPlayer = GetPlayer(isOwn);
        BaseComponent component = targetPlayer.leader;
        effect.ExecuteEffect(component);
    }

    public override void SuperEvolve(bool isOwn)
    {
        // 自分のリーダーを2ではなく4回復
        HealEffect effect = new HealEffect(new List<int> { 4 });
        var targetPlayer = GetPlayer(isOwn);
        BaseComponent component = targetPlayer.leader;
        effect.ExecuteEffect(component);
    }
}
