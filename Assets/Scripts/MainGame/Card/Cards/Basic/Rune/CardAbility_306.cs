using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_306 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // 場のフォロワーすべてに2ダメージ。【土の秘術_1】自分のデッキから1枚を引く。
        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, Field.FieldType.ALL);
        if (targetCard.Count > 0)
        {
            DamageEffect damageEffect = new DamageEffect(new List<int>{ 2 });
            List<BaseComponent> components = new List<BaseComponent>();
            foreach(var card in targetCard)
            {
                BaseComponent component = card;
                components.Add(component);
            }
            damageEffect.ExecuteEffect(components);
        }  
        //if
        DrawEffect drawEffect = new DrawEffect(new List<int>{1});
        drawEffect.ExecuteEffect(targetCard);
        EarthSigleEffect earthSigleEffect = new EarthSigleEffect(new List<int>{-1});
        earthSigleEffect.ExecuteEffect();
    }
}
