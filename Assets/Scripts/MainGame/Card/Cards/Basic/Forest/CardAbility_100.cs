using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_100 : BaseCardAbility
{
    private const int FAIRY_ID = 107; 

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // フェアリーを二枚手札に加える

    }
}
