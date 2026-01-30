using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseAbilityEffect : BaseEffect
{
    public LoseAbilityEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard)
    {
        if (param == null)
        {
            targetCard.ClearAllAbility();
        }
        else
        {
            targetCard.RemoveKeyword((GameEnum.KeywordAbility)param[0]);
        }
    }
}
