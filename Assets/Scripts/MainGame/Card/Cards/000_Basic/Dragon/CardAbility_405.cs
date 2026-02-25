using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_405 : BaseCardAbility
{
    private const int VASTWING_DRAGON_ID = 407;
    private const int ENHANCE_COST = 7;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Enhance, null, ENHANCE_COST));
    }

    public override void Enhance(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { VASTWING_DRAGON_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
