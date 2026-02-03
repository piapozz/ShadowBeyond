using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_303 : BaseCardAbility
{
    private const int ENGAGE_COST = 1;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.EarthSigle));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Aura));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.NoDestroy));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Fanfare(bool isOwn)
    {
        var targetPlayer = GetPlayer(isOwn);
        DrawEffect drawEffect = new DrawEffect(new List<int>{1});
        drawEffect.ExecuteEffect(targetPlayer.deck);
    }

    public override void Engage(bool isOwn)
    {
        EarthSigleEffect enterCardFieldEffect = new EarthSigleEffect(new List<int>{1});
        enterCardFieldEffect.ExecuteEffect();
    }
}
