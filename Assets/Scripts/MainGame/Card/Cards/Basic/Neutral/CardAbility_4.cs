using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_4 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Evolve(bool isOwn)
    {
        // 自分のリーダーを2回復
    }

    public override void SuperEvolve(bool isOwn)
    {
        // 自分のリーダーを2ではなく4回復
    }
}
