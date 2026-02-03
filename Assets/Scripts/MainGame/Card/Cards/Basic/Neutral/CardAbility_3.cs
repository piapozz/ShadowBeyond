using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_3 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Engage(bool isOwn)
    {
        // これを破壊
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect();
        // 相手の場のをフォロワーを1体選ぶ。守護を失う 
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => 
        {
            return (null != card.GetKeywordAbility(GameEnum.KeywordAbility.Ward));
        }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        LoseAbilityEffect loseAbilityEffect = new LoseAbilityEffect(new List<int>{ (int)GameEnum.KeywordAbility.Ward });
        loseAbilityEffect.ExecuteEffect(targetCard);
    }
}
