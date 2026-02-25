using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_703 : BaseCardAbility
{
    private const int GEAR_OF_AMBITION_ID = 716;
    private const int GEAR_OF_REMEMBRANCE_ID = 716;
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
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { GEAR_OF_AMBITION_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
        addCardHandEffect = new AddCardHandEffect(new List<int> { GEAR_OF_REMEMBRANCE_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
