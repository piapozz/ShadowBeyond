using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_402 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetPlayer = GetPlayer(!isOwn);
        DamageEffect damageEffect = new DamageEffect(new List<int> { 6 });
        BaseComponent component = targetPlayer.leader;
        damageEffect.ExecuteEffect(component);
    }
}
