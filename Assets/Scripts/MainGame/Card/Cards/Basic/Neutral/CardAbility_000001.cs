using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_000001 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward, null));
    }

    public override void Enhance(bool isOwn, List<BaseComponent> selected = null)
    {
        // effect‚É“n‚·
        BuffEffect effect = new BuffEffect(new List<int> { 3, 3 });
        effect.ExecuteEffect(sourceData);
    }

    public override void LastWord(bool isOwn)
    {
        // ˆê–‡ˆø‚­
        DrawEffect effect = new DrawEffect(new List<int>{ 1 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck);
    }

    public override void Evolve(bool isOwn, List<BaseComponent> selected = null)
    {
        // ˆê–‡ˆø‚­
        DrawEffect effect = new DrawEffect(new List<int>{ 1 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck);
    }

    public override void SuperEvolve(bool isOwn, List<BaseComponent> selected = null)
    {
        // ˆê–‡ˆø‚­
        DrawEffect effect = new DrawEffect(new List<int> { 1 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck);
    }
}
