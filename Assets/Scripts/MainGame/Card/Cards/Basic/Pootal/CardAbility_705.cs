using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_705 : BaseCardAbility
{
    private const int MECHANZED_BEAST = 705;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));
    }

    public override void Evolve(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { MECHANZED_BEAST, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }

    public override void SuperEvolve(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { MECHANZED_BEAST, 2 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
