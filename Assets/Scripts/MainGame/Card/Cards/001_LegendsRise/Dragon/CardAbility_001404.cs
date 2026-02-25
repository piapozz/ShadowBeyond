using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【守護】
//【ラストワード】『大翼のドラゴン』1枚を自分の場に出す。
public class CardAbility_001404 : BaseCardAbility
{
    private const int VASTWING_DRAGON_ID = 407;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward, null));
    }

    public override void LastWord(bool isOwn)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { VASTWING_DRAGON_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}

