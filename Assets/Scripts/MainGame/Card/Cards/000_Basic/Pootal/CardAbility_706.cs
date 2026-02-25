using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_706 : BaseCardAbility
{
    private const int COUNTDOWN_TURNS = 3;
    private const int PUPPET_ID = 707;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Countdown, null, COUNTDOWN_TURNS));

        // 自分のターン終了時、『操り人形』1枚を自分の手札に加える。
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        AddCardHandEffect addCardHandEffect = new AddCardHandEffect(new List<int> { PUPPET_ID, 1 });
        addCardHandEffect.ExecuteEffect(GetPlayer(isOwn).hand);
    }
}
