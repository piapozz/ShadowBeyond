using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_400 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 1 });
        BaseComponent component = targetCard;
        damageEffect.ExecuteEffect(component);
    }
}
