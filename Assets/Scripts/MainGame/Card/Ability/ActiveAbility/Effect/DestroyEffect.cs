using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : BaseEffect
{
    public DestroyEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        if (targetCard.HaveKeyword(GameEnum.KeywordAbility.NoDestroy)) return;
        targetCard.Destroy();
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        for (int i = 0, max = targetCards.Count; i < max; i++)
        {
            ExecuteEffect(targetCards[i]);
        }
    }
}
