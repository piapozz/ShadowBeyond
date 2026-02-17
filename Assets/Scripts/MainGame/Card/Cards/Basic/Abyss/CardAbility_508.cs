using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_508 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
        // 自分のターン終了時これを消滅
        activeAbilities.Add(new ActiveAbility(AbilityManager.TriggerTiming.OwnTurnEnd,
           null,
           new BanishEffect(null),
           null,
           ActiveAbility.Zone.Field,
           sourceData.GetObject().isLocal,
           sourceData));
    }

    public override void LeaveField(bool isOwn)
    {
        // 場を離れる場合、これを消滅。
        BanishEffect banishEffect = new BanishEffect(null);
        banishEffect.ExecuteEffect(sourceData);
    }
}
