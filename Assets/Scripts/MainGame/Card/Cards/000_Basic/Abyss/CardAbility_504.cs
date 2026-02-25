using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_504 : BaseCardAbility
{
    private const int BAT_ID = 507;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void LastWord(bool isOwn)
    {
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { BAT_ID, 1});
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
