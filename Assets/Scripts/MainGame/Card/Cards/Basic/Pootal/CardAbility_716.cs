using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_716 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Aura));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));
    }

    public override void Fanfare(bool isOwn)
    {
        var targetCard = BattleManager.instance.field.GetCards((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard.Count <= 0) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 5 });
        damageEffect.ExecuteEffect(targetCard);
        HealEffect healEffect = new HealEffect(new List<int> { 5 });
        BaseComponent component = GetPlayer(isOwn).leader;
        healEffect.ExecuteEffect(component);
    }
}
