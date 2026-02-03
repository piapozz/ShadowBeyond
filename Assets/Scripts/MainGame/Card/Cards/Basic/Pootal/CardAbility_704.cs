using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_704 : BaseCardAbility
{
    private const int GEAR_OF_REMEMBRANCE_ID = 716;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }
    public override void Fanfare(bool isOwn)
    {
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { GEAR_OF_REMEMBRANCE_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
