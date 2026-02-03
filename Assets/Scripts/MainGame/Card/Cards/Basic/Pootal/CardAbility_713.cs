using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_713 : BaseCardAbility
{
    private const int OMINOUS_ARTIFACT_BETA_ID = 714;
    private const int OMINOUS_ARTIFACT_GAMMA_ID = 715;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;

        //【融合】『デストロイアーティファクトβ』や『デストロイアーティファクトγ』
        // これに【融合】したとき、これに【融合】した種類が2なら、これは『イクシードアーティファクトΩ』に変身する。
        // 自分のターン終了時、自分のリーダーを3回復。
    }
}
