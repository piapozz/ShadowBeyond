using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_203 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        base.Initialize(setCard);
    }

    public override void Fanfare(bool isOwn)
    {
        // 自分の手札1枚を選ぶ。それをデッキに戻す。自分のデッキからロイヤル・フォロワー2枚を引く。
    }
}
