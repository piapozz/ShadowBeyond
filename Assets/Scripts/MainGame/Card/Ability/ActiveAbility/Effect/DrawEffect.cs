using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BaseEffect
{
    public DrawEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(Deck targetDeck, Func<CardData, bool> condition = null)
    {
        List<CardData> drawCards = new List<CardData>();
        // 条件がないなら普通にドロー
        if (condition == null)
            return targetDeck.DrawDeck(param[0]);
        // 条件があるなら条件に合うカードをドロー
        else
            return targetDeck.DrawDeck(condition);
    }
}
