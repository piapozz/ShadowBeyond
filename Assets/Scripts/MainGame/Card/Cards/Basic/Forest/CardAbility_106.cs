using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_106 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // 自分の場のカード1枚を選ぶ。それを手札に戻す。相手の場のフォロワーからランダム1枚に2ダメージ。
        var targetOwnCard  = BattleManager.instance.field.GetRandomCard((card) => { return card != null; }, isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetOwnCard == null ) return;
        BounceEffect bounceEffect = new BounceEffect(null);
        bounceEffect.ExecuteEffect(targetOwnCard);
        var targetOpponentCard = BattleManager.instance.field.GetRandomCard((card) => {return card.type == GameEnum.CardType.FOLLOWER; }, ! isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetOpponentCard == null) return;
        DamageEffect damageEffect = new DamageEffect(new List<int>{2});
        BaseComponent component = targetOpponentCard;
        damageEffect.ExecuteEffect(component);
    }
}
