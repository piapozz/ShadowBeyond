using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 場のフォロワーすべてに5ダメージ。
public class CardAbility_001405 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, Field.FieldType.ALL);
        if (targetCard.Count > 0)
        {
            DamageEffect damageEffect = new DamageEffect(new List<int> { 5 });
            List<BaseComponent> components = new List<BaseComponent>();
            foreach (var card in targetCard)
            {
                BaseComponent component = card;
                components.Add(component);
            }
            damageEffect.ExecuteEffect(components);
        }
    }
}

