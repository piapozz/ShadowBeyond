using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_104 : BaseCardAbility
{
    private const int COMBO_COST = 3;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // 【コンボ_3】相手の場のフォロワー1枚を選ぶ。それに3ダメージ。
    }
}
