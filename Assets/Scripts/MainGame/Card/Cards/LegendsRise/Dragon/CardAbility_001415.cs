using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コスト3【アクト】『乙姫お守り隊』1枚を自分の場に出す。自分の手札1枚を選ぶ。それを捨てる。
public class CardAbility_001415 : BaseCardAbility
{
    private const int ENGAGE_COST = 3;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
        TargetCondition condition = TargetCondition.Any;
        condition.type.Add(GameEnum.CardType.FOLLOWER);
        selectTarget[(int)TargetTiming.Engage] = new Target(Target.TargetSide.Opponent, Target.TargetZone.Field, condition, 1);
    }

    public override void Engage(bool isOwn, List<BaseComponent> selected = null)
    {
        base.Engage(isOwn, selected);
        // これを破壊
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect(sourceData);
        // 選んだフォロワーの守護を失わせる
        if (selected == null) return;

        LoseAbilityEffect loseAbilityEffect = new LoseAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Ward });
        loseAbilityEffect.ExecuteEffect(selected[0] as CardData);
    }
}
