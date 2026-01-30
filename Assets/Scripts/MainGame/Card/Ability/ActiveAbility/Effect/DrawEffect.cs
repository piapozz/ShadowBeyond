using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BaseEffect
{
    public DrawEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(Deck targetDeck, Func<CardData, bool> condition = null)
    {
        // 条件がないなら普通にドロー
        if (condition == null)
            targetDeck.DrawDeck(param[0]);
        // 条件があるなら条件に合うカードをドロー
        else
        {
            List<CardData> drawCard = targetDeck.GetCards(condition);
            targetDeck.DrawDeck(condition);
        }
    }
}
