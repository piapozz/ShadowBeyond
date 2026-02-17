using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_005209 : BaseCardAbility
{
    const int DAMAGE_VALUE = 7;
    const int DAMAGE_COUNT = 3;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
        // 自分のターン終了時、これが進化前なら、相手の場のフォロワーすべてに7ダメージ。進化後なら、「場の他のフォロワーか自分のリーダーか相手のリーダーからランダム1枚に7ダメージ。」を3回行う。

        TargetCondition condition = TargetCondition.Any;
        condition.type.Add(GameEnum.CardType.FOLLOWER);
        // 進化前
        activeAbilities.Add(new ActiveAbility(AbilityManager.TriggerTiming.OwnTurnEnd,
           new Target(Target.TargetSide.Opponent, Target.TargetZone.Field, condition),
           new DamageEffect(new List<int> { DAMAGE_VALUE }),
           () => !sourceData.isAnyEvolved,
           ActiveAbility.Zone.Field,
           sourceData.GetObject().isLocal,
           sourceData));

        // 進化後
        ActiveAbility evolvedAbility =
        new ActiveAbility(AbilityManager.TriggerTiming.OwnTurnEnd,
           new Target(Target.TargetSide.Both, Target.TargetZone.FieldAndLeader, condition, 1, true),
           new DamageEffect(new List<int> { DAMAGE_VALUE }),
           () => sourceData.isAnyEvolved,
           ActiveAbility.Zone.Field,
           sourceData.GetObject().isLocal,
           sourceData);
        for (int i = 0; i < DAMAGE_COUNT; i++)
        {
            activeAbilities.Add(evolvedAbility);
        }
    }
}
