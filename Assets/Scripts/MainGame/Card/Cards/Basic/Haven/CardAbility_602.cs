using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_602 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));
    }

    public override void Fanfare(bool isOwn)
    {
        var targetPlayer = GetPlayer(isOwn);
        HealEffect healEffect = new HealEffect(new List<int> { 5 });
        BaseComponent component = targetPlayer.leader;
        healEffect.ExecuteEffect(component);
    }
}
