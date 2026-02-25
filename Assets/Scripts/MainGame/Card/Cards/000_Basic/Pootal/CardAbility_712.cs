using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_712 : BaseCardAbility
{
    private const int OMINOUS_ARTIFACT_ALPHA_ID = 713;
    private const int OMINOUS_ARTIFACT_BETA_ID = 714;
    private const int OMINOUS_ARTIFACT_GAMMA_ID = 715;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));

        //【融合】アーティファクト・カード
        // これに【融合】したカードのコストの合計によって変身する。
        // 1 ⇒『デストロイアーティファクトα』
        // 2 ⇒『デストロイアーティファクトβ』
        // 3以上⇒『デストロイアーティファクトγ』
    }
}
