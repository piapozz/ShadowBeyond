using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 【ファンファーレ】相手の場のフォロワーすべては-0/-9する。
// 【超進化時】自分のデッキから3枚を引く。

public class CardAbility_001413 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, !isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard.Count <= 0) return;
        BuffEffect buffEffect = new BuffEffect(new List<int> { 0, -9 });

        List<BaseComponent> components = new List<BaseComponent>();
        BaseComponent component;
        foreach (var card in targetCard)
        {
            component = card;
            components.Add(component);
        }
        buffEffect.ExecuteEffect(components);
    }

    public override void SuperEvolve(bool isOwn, List<BaseComponent> selected = null)
    {
        DrawEffect effect = new DrawEffect(new List<int> { 3 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck);
    }
}
