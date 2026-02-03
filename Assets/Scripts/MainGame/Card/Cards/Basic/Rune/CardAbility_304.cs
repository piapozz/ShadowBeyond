using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class CardAbility_304 : BaseCardAbility
{
    private const int GUARDIAN_GOLEM_ID = 308;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        //if (BattleManager.instance.field.Get)
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int>{ GUARDIAN_GOLEM_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
        EarthSigleEffect earthSigleEffect = new EarthSigleEffect(new List<int>(-1));
        earthSigleEffect.ExecuteEffect();
    }

    public override void SuperEvolve(bool isOwn)
    {
        // 【超進化時】自分の場のゴーレム・フォロワー1枚を選ぶ。それは進化する。それは+3/+3する。
        var targetCard = BattleManager.instance.field.GetRandomCard(
            (card) => card.typeDetail != null && card.typeDetail.Contains(CardTypeDetail.GOLEM), ! isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        EvolveEffect evolveEffect = new EvolveEffect(null);
        evolveEffect.ExecuteEffect(targetCard);
        BuffEffect buffEffect = new BuffEffect(new List<int>{3, 3});
        buffEffect.ExecuteEffect();
    }
}
