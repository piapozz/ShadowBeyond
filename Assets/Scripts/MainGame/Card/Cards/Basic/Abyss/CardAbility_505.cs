using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_505 : BaseCardAbility
{
    private const int GHOST_ID = 508; 
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Evolve(bool isOwn)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { GHOST_ID, 2 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }

    public override void SuperEvolve(bool isOwn)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { GHOST_ID, 2 });
        var enterList = enterCardFieldEffect.ExecuteEffect(isOwn);
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Drane });
        giveKeywordAbilityEffect.ExecuteEffect(enterList);
    }
}
