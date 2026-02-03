using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_205 : BaseCardAbility
{
    private const int RUSTY_ID = 205;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }
    public override void SuperEvolve(bool isOwn)
    {
        // 【超進化時】自分のデッキから『魔煌のトリックスター・ラスティ』すべてを引く。それは【疾走】を持つ。
        var targetPlayer = GetPlayer(isOwn);
        var drawCount = targetPlayer.deck.GetCards((card) => { return card.id == RUSTY_ID; }).Count;
        DrawEffect drawEffect = new DrawEffect(new List<int>{drawCount});
        var drawList = drawEffect.ExecuteEffect(targetPlayer.deck, (card) => { return card.id == RUSTY_ID; });
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Storm });
        giveKeywordAbilityEffect.ExecuteEffect(drawList);
    }
}
