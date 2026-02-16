using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_000003 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
        TargetCondition condition = TargetCondition.Any;
        condition.type.Add(GameEnum.CardType.FOLLOWER);
        selectTarget[(int)TargetTiming.Engage] = new Target(Target.TargetSide.Opponent, Target.TargetZone.Field, condition, 1);
    }

    public override void Engage(bool isOwn, List<BaseFieldObject> selected = null)
    {
        base.Engage(isOwn);
        // これを破壊
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect(sourceData);
        // 選んだフォロワーの守護を失わせる
        if (selected == null) return;

        LoseAbilityEffect loseAbilityEffect = new LoseAbilityEffect(new List<int>{ (int)GameEnum.KeywordAbility.Ward });
        loseAbilityEffect.ExecuteEffect(selected[0]);
    }
}
