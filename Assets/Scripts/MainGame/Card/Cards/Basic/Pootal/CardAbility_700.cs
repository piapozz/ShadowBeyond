using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_700 : BaseCardAbility
{
    private const int ENHANCED_PUPPET_ID = 708;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { ENHANCED_PUPPET_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
