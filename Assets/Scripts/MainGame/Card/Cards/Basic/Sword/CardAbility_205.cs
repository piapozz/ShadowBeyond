using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_205 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }
    public override void SuperEvolve(bool isOwn)
    {
        // 【超進化時】自分のデッキから『魔煌のトリックスター・ラスティ』すべてを引く。それは【疾走】を持つ。
    }
}
