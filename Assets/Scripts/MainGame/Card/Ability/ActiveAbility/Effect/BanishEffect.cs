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
        UIManager.instance.RemoveFieldCard(targetCard.GetObject());
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        targetCards.ForEach((card) =>
        {
            card.Banish();
            UIManager.instance.RemoveFieldCard(card.GetObject());
        });
    }
}
