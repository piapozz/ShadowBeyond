using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_714 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        // 自分のターン終了時、相手のリーダーに3ダメージ。
    }
}
