using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_005209 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
        // 自分のターン終了時、これが進化前なら、相手の場のフォロワーすべてに7ダメージ。進化後なら、「場の他のフォロワーか自分のリーダーか相手のリーダーからランダム1枚に7ダメージ。」を3回行う。
        activeAbilities.Add(new ActiveAbility(AbilityManager.TriggerTiming.OwnTurnEnd,
           null,
           new DamageEffect(new List<int> { 7 }),
           null,
           ActiveAbility.Zone.Field,
           sourceData.GetObject().isLocal,
           sourceData));
    }
}
