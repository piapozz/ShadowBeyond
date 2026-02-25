using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 【超進化時】自分の場の他のフォロワー1枚を選ぶ。それは【疾走】を持つ。
public class CardAbility_001407 : BaseCardAbility
{

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER && card != sourceData; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Intimidate });
        giveKeywordAbilityEffect.ExecuteEffect(targetCard);
    }
}
