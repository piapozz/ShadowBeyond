using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_604 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Evolve(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) =>
        {
            return card.GetCurrentStatus().m_defance <= 3;
        }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        // Á–Å
    }

    public override void SuperEvolve(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetCards((card) =>
        {
            return card.GetCurrentStatus().m_defance <= 3;
        }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
    }
}
