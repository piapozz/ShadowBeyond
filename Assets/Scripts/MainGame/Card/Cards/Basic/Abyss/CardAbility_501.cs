using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_501 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        var targetPlayer  = GetPlayer(isOwn);
        DamageEffect damageEffect = new DamageEffect(new List<int> { 1 });
        BaseComponent component = targetPlayer.leader;
        damageEffect.ExecuteEffect(component);
    }
}
