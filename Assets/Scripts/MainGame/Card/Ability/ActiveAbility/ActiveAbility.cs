using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityManager;

public class ActiveAbility
{
    public enum Zone
    {
        Deck = 0,
        Hand,
        Crest,
        Field
    }

    public TriggerTiming trigger { get; private set; }
    public Target target { get; private set; }
    public BaseEffect effect { get; private set; }
    public BaseCondition condition { get; private set; }
    public Zone zone { get; private set; }
    public CardData sourceCard { get; private set; }

    public ActiveAbility(TriggerTiming setTiming, Target setTarget, BaseEffect setEffect, 
        BaseCondition setCondition, Zone setZone, CardData setSourceCard = null)
    {
        trigger = setTiming;
        target = setTarget;
        effect = setEffect;
        condition = setCondition;
        zone = setZone;
        sourceCard = setSourceCard;
    }
}
