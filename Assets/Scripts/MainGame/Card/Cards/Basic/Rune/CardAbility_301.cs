using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_301 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        var targetPlayer = GetPlayer(isOwn);
        DrawEffect drawEffect = new DrawEffect(new List<int>{1 });
        drawEffect.ExecuteEffect(targetPlayer.hand);
    }
}
