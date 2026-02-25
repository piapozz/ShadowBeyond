using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_500 : BaseCardAbility
{
    private const int NECROMANCY_COST = 4;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // 【ファンファーレ】【ネクロマンス_4】これは【疾走】を持つ。
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int> { (int)GameEnum.KeywordAbility.Storm });
        giveKeywordAbilityEffect.ExecuteEffect(sourceData);
    }
}
