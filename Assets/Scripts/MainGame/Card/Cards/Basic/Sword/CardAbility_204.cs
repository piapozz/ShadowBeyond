using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_204 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void LastWord(bool isOwn)
    {
        // 【ラストワード】『ナイト』1枚を自分の場に出す。
    }
}
