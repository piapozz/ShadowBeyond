using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_001200 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush));
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // •KŽE‚ð•t—^
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Bane });
        giveKeywordAbilityEffect.ExecuteEffect(sourceData);
    }
}
