using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_Test : BaseCardAbility
{
    public override void Initialize()
    {
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));
    }
}
