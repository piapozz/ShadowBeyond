using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : BaseEffect
{
    public BuffEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        targetCard.AddStatus(param[0], param[1]);
    }

    public override void ExecuteEffect(List<CardData> targetCard, CardData sourceCard = null)
    {
        for (int i = 0, max = targetCard.Count; i < max; i++)
        {
            targetCard[i].AddStatus(param[0], param[1]);
        }
    }
}
