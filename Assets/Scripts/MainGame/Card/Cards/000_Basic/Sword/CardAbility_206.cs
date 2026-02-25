using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_206 : BaseCardAbility
{
    private const int COUNTDOWN_TURNS = 3;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Countdown, null, COUNTDOWN_TURNS));

        // 自分のフォロワーが場に出たとき、それは+1/+1する。
        activeAbilities.Add(new ActiveAbility(AbilityManager.TriggerTiming.OwnEnterField,
           null,
           new BuffEffect(new List<int> { 1 ,1 }),
           null,
           ActiveAbility.Zone.Field,
           sourceData.GetObject().isLocal,
           sourceData));
    }
}
