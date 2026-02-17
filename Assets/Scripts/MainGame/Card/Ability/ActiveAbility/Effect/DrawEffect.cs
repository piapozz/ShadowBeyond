using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BaseEffect
{
    public DrawEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        Deck targetDeck = context.player.deck;
        return ExecuteEffect(targetDeck);
    }

    public override List<CardData> ExecuteEffect(Deck targetDeck, Func<CardData, bool> condition = null)
    {
        // 条件がないなら普通にドロー
        if (condition == null)
            return targetDeck.DrawDeck(param[0]);
        // 条件があるなら条件に合うカードをドロー
        else
        {
            if (param == null)
                return targetDeck.DrawDeck(condition);
            else
                return targetDeck.DrawDeck(condition, param[0]);
        }
    }
}
