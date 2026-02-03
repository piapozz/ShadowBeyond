using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanishEffect : BaseEffect
{
    public BanishEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        targetCard.Banish();
        targetCard.GetObject().SetCardState(CardObject.CardState.UNUSE);
        UIManager.instance.RemoveFieldCard(targetCard.GetObject());
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        for (int i = 0, max = targetCards.Count; i < max; i++)
        {
            ExecuteEffect(targetCards[i]);
        }
    }
}
