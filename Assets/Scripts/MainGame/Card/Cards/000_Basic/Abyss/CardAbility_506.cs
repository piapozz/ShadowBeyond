using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_506 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect(targetCard);
        DrawEffect drawEffect = new DrawEffect(new List<int> { 2 });
        drawEffect.ExecuteEffect(GetPlayer(isOwn).deck);
    }
}
