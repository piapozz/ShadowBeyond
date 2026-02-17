using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public List<BaseComponent> targets { get; private set; }
    public CardData sourceCard { get; private set; }
    public bool isOwn { get; private set; }
    public BattleManager.Player player { get; private set; }
    public Func<CardData, bool> condition { get; private set; }

    public EffectContext(
        List<BaseComponent> setTargets,
        CardData setSource,
        bool setIsOwn,
        BattleManager.Player setPlayer = default,
        Func<CardData, bool> setCondition = null)
    {
        targets = setTargets;
        sourceCard = setSource;
        isOwn = setIsOwn;
        player = setPlayer;
        condition = setCondition;
    }
}

public abstract class BaseEffect
{
    protected List<int> param = null;

    public BaseEffect(List<int> setParam = null)
    {
        param = setParam;
    }

    public abstract List<CardData> ExecuteEffect(EffectContext context);
    public virtual void ExecuteEffect() { }
    public virtual List<CardData> ExecuteEffect(bool isOwn) { return null; }
    public virtual void ExecuteEffect(CardData targetCard, CardData sourceCard = null) { }
    public virtual void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null) { }
    public virtual void ExecuteEffect(BaseComponent targetComponent) { }
    public virtual void ExecuteEffect(List<BaseComponent> targetComponents) { }
    public virtual List<CardData> ExecuteEffect(Deck targetDeck, Func<CardData, bool> condition = null) { return null; }
    public virtual List<CardData> ExecuteEffect(Hand targetHand) { return null; }
    public virtual void ExecuteEffect(Leader targetLeader) { }
}
