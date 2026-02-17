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

    // 発動タイミング
    public TriggerTiming timing { get; private set; }
    // ターゲット
    public Target target { get; private set; }
    // 発動能力
    public BaseEffect effect { get; private set; }
    // 発動条件
    public Func<bool> condition { get; private set; }
    // 発動するエリア
    public Zone zone { get; private set; }
    // 自身の能力か否か
    public bool isOwn { get; private set; }
    // 発動元のカード
    public CardData sourceCard { get; private set; }
    // 対象のプレイヤー
    public BattleManager.Player player { get; private set; }
    // 詳細な条件
    public Func<CardData, bool> detailCondition { get; private set; }

    public ActiveAbility(TriggerTiming setTiming, Target setTarget, BaseEffect setEffect,
        Func<bool> setCondition, Zone setZone, bool setIsOwn, CardData setSourceCard, BattleManager.Player setPlayer = default, Func<CardData, bool> setDetailCondition = null)
    {
        timing = setTiming;
        target = setTarget;
        effect = setEffect;
        condition = setCondition;
        zone = setZone;
        isOwn = setIsOwn;
        sourceCard = setSourceCard;
        player = setPlayer;
        detailCondition = setDetailCondition;
    }
}
