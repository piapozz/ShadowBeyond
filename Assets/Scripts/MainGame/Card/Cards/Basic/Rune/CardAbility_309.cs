using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_309 : BaseCardAbility
{
    private const int ENGAGE_COST = 1;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard; keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.EarthSigle));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Aura));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.NoDestroy));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Engage(bool isOwn, List<BaseFieldObject> selected = null)
    {
        EarthSigleEffect enterCardFieldEffect = new EarthSigleEffect(new List<int>{1});
        enterCardFieldEffect.ExecuteEffect();
    }
}
