using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【ファンファーレ】自分の手札1枚を選ぶ。それを捨てる。相手の場のフォロワーすべてにXダメージ。Xは選んだカードのコストである。
//【超進化時】相手は『クレスト：灼熱のアナテマ・バーンドナイト』を持つ。
public class CardAbility_001417 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        int damage = 0;
        var targgetHandList = GetPlayer(isOwn).hand.GetCards((card) => { return card != null; });
        var targetHand = targgetHandList[BattleManager.instance.rand.Next(0, targgetHandList.Count - 1)];
        if (targetHand == null) return;
        damage = targetHand.cost;

        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard.Count <= 0) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 5 });

        List<BaseComponent> components = new List<BaseComponent>();
        BaseComponent component;
        foreach (var card in targetCard)
        {
            component = card;
            components.Add(component);
        }
        damageEffect.ExecuteEffect(components);
    }
}
