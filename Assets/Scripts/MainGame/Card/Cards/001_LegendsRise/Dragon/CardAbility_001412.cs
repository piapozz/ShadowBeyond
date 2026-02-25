using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 【ファンファーレ】『大翼のドラゴン』1枚を自分の場に出す。
// 【守護】
public class CardAbility_001412 : BaseCardAbility
{
    private const int VASTWING_DRAGON_ID = 407;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward, null));
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { VASTWING_DRAGON_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }
}
