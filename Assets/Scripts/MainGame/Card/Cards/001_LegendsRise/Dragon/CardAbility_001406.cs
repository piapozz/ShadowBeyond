using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// yƒtƒ@ƒ“ƒtƒ@[ƒŒzyŠoÁz‚È‚çA‚±‚ê‚ÍyˆĞˆ³z‚ğ‚ÂB
// y¾‘–z
public class CardAbility_001406 : BaseCardAbility
{
    private const int VASTWING_DRAGON_ID = 407;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm, null));
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        if (!GetPlayer(isOwn).leader.IsOverflow()) return;
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Intimidate });
        giveKeywordAbilityEffect.ExecuteEffect(sourceData);
    }
}
