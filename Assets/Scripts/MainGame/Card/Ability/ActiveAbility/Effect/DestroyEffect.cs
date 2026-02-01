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
        targetCard.Destroy();
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        targetCards.ForEach(card => card.Destroy());
    }
}
