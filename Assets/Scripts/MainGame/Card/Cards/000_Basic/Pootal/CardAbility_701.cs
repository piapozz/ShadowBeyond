using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_701 : BaseCardAbility
{
    private const int GEAR_OF_AMBITION_ID = 716;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush));
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { GEAR_OF_AMBITION_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
