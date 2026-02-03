using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_601 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        var targetPlayer = GetPlayer(isOwn);
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        BuffEffect buffEffect = new BuffEffect(new List<int> { 1, 1 });
        buffEffect.ExecuteEffect(targetCard);
    }

    public override void Evolve(bool isOwn)
    {
        Fanfare(isOwn);
    }

    public override void SuperEvolve(bool isOwn)
    {
        Fanfare(isOwn);
    }
}
