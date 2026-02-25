using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// yƒtƒ@ƒ“ƒtƒ@[ƒŒzyŠoÁz‚È‚çA‚±‚ê‚Íy¾‘–z‚ğ‚ÂB
public class CardAbility_001403 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        if (!GetPlayer(isOwn).leader.IsOverflow()) return;
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Storm });
        giveKeywordAbilityEffect.ExecuteEffect(sourceData);
    }
}