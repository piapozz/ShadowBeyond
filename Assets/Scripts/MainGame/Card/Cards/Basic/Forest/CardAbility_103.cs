using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_103 : BaseCardAbility
{
    private const int COUNTDOWN_TURNS = 2;
    private const int FAIRY_ID = 107;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Countdown, null, COUNTDOWN_TURNS));

        // 自分の妖精・フォロワーが場に出たとき、相手の場のフォロワーからランダム1枚に1ダメージ。
        activeAbilities.Add(new ActiveAbility(AbilityManager.TriggerTiming.OwnEnterField,
            new Target(Target.TargetSide.Opponent, Target.TargetZone.Field, new TargetCondition(), 1, true),
            new DamageEffect(new List<int> { 1 }),
            null,
            ActiveAbility.Zone.Field,
            setCard.GetCardObject().isLocal,
            sourceData));
    }

    public override void Fanfare(bool isOwn)
    {
        // フェアリーを一枚手札に加える
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int>{ FAIRY_ID , 1});
        var targetHand = GetPlayer(isOwn).hand;
        addCardHandEffect.ExecuteEffect(targetHand);
    }

}
