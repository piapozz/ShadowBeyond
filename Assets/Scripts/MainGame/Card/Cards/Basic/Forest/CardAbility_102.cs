using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_102 : BaseCardAbility
{
    private const int COMBO_COST = 3;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public　override void Fanfare(bool isOwn)
    {
        // コンボ3 進化
    }

    public override void Attack(bool isOwn)
    {
        // 自分のリーダー2回復
    }
}
