using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_404 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Evolve(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 4 });
        BaseComponent component = targetCard;
        damageEffect.ExecuteEffect(component);
    }

    public override void SuperEvolve(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard.Count <= 0) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 4 });
        List<BaseComponent> components = null;
        foreach (var card in targetCard)
        {
            BaseComponent component = card;
            components.Add(component);
        }
        damageEffect.ExecuteEffect(components);
    }
}
