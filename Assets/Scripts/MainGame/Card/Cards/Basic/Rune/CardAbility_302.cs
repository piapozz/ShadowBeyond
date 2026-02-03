using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_302 : BaseCardAbility
{
    private const int CLAYGOLEM_ID = 307;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int>{CLAYGOLEM_ID, 1});
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
