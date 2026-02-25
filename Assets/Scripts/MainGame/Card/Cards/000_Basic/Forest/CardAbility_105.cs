using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_105 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
    }

    public override void SuperEvolve(bool isOwn, List<BaseComponent> selected = null)
    {
        // 相手の場のフォロワー1枚を選ぶ。それを手札に戻す。
        var TargetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (TargetCard == null) return;
        BounceEffect bounceEffect = new BounceEffect(null);
        bounceEffect.ExecuteEffect(TargetCard);
    }
}
