using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_603 : BaseCardAbility
{
    private const int ENGAGE_COST = 2;
    private const int REGALFALCON_ID =  608;
    private const int COUNTDOWN_TURNS = 2;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Countdown, null, COUNTDOWN_TURNS));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void LastWord(bool isOwn)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { REGALFALCON_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
