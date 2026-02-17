using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_104 : BaseCardAbility
{
    private const int COMBO_COST = 3;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        TargetCondition condition = TargetCondition.Any;
        condition.type.Add(GameEnum.CardType.FOLLOWER);
        selectTarget[(int)TargetTiming.Fanfare] = new Target(Target.TargetSide.Opponent, Target.TargetZone.Field, condition, 1);
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // 【コンボ_3】相手の場のフォロワー1枚を選ぶ。それに3ダメージ。
        //if (targetPlayer.leader.comboCount < COMBO_COST) return;
        if (selected == null) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 3 });
        damageEffect.ExecuteEffect(selected);
    }
}
