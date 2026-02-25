using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 【ファンファーレ】自分の場の他のフォロワー1枚を選ぶ。それは+2/+2する。【覚醒】なら、+2/+2ではなく+3/+3。
public class CardAbility_001408 : BaseCardAbility
{

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER && card != sourceData; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        BuffEffect effect = new BuffEffect(new List<int> { 2, 2 });
        if (GetPlayer(isOwn).leader.IsOverflow()) effect = new BuffEffect(new List<int> { 3, 3 });
        effect.ExecuteEffect(sourceData);
    }
}
