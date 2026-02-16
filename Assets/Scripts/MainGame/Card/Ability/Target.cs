using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommonModule;
using static GameEnum;
using static CardData;

public struct TargetCondition
{
    public int? ID;
    public List<CardType> type;
    public List<LeaderClass> leaderClass;
    public List<CardTypeDetail> cardTypeDetail;

    public EvolveState evolveState;

    public IntRange attack;
    public IntRange defence;

    public bool? isHurt;

    public static TargetCondition Any => new TargetCondition
    {
        ID = null,
        type = new List<CardType>(),
        leaderClass = new List<LeaderClass>(),
        cardTypeDetail = new List<CardTypeDetail>(),
        evolveState = EvolveState.None,
        attack = IntRange.Any,
        defence = IntRange.Any,
        isHurt = null
    };
}

public class Target
{
    public enum TargetSide
    {
        Own = 0,
        Opponent,
        Both
    }

    public enum TargetZone
    {
        Hand = 0,
        Field,
        Leader,
        FieldAndLeader
    }

    public TargetSide targetSide;
    public TargetZone targetZone;
    public TargetCondition condition;
    public int count;
    public bool isRandom;

    public Target()
    {
        targetSide = TargetSide.Own;
        targetZone = TargetZone.Field;
        condition = TargetCondition.Any;
        count = 1;
        isRandom = false;
    }

    public Target(TargetSide setSide, TargetZone setZone, TargetCondition setCondition, int setCount = 0, bool setRandom = false)
    {
        targetSide = setSide;
        targetZone = setZone;
        condition = setCondition;
        count = setCount;
        isRandom = setRandom;
    }
}
