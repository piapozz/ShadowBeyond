using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_403 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        int damade = 2;
        if (GetPlayer(isOwn).leader.IsOverflow()) damade = 4;
        DamageEffect damageEffect = new DamageEffect(new List<int> { damade });
        BaseComponent component = targetCard;
        damageEffect.ExecuteEffect(component);
    }
}
