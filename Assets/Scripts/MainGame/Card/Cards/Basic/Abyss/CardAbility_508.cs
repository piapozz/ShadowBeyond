using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_508 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
    }

    public override void LeaveField(bool isOwn)
    {
        // 場を離れる場合、これを消滅。
        // 自分のターン終了時、これを消滅。
    }
}
