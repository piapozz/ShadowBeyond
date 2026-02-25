using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_710 : BaseCardAbility
{
    private const int FORTIFIER_ARTIFACT_ID = 712;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        //【融合】アーティファクト・アミュレット
        //これに【融合】したとき、これは『キャッスルアーティファクト』に変身する。
        //プレイできない。
    }
}
