using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect
{
    protected List<int> param = null;

    public BaseEffect(List<int> setParam = null)
    {
        param = setParam;
    }

    public virtual void ExecuteEffect() { }
    public virtual List<CardData> ExecuteEffect(bool isOwn) { return null; }
    public virtual void ExecuteEffect(CardData targetCard, CardData sourceCard = null) { }
    public virtual void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null) { }
    public virtual void ExecuteEffect(BaseComponent targetComponent) { }
    public virtual void ExecuteEffect(List<BaseComponent> targetComponents) { }
    public virtual List<CardData> ExecuteEffect(Deck targetDeck, Func<CardData, bool> condition = null) { return null; }
    public virtual void ExecuteEffect(Hand targetHand) { }
    public virtual void ExecuteEffect(Leader targetLeader) { }
}
