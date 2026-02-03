using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_204 : BaseCardAbility
{
    private const int KNIGHT_ID = 207;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void LastWord(bool isOwn)
    {
        // 【ラストワード】『ナイト』1枚を自分の場に出す。
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { KNIGHT_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
