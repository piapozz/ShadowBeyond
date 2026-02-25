using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_709 : BaseCardAbility
{
    private const int SRIKER_ARTIFACT_ID = 711;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        //【融合】アーティファクト・アミュレット
        //これに【融合】したとき、これは『アタックアーティファクト』に変身する。
        //プレイできない。
    }
}
