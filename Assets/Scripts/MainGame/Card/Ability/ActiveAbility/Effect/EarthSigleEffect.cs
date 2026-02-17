using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommonModule;

public class EarthSigleEffect : BaseEffect
{
    public EarthSigleEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        return ExecuteEffect(context.isOwn);
    }

    public override List<CardData> ExecuteEffect(bool isOwn)
    {
        // ƒtƒB[ƒ‹ƒh‚©‚ç“y‚Ìˆó‚ğæ“¾
        List<CardData> earthSigleCard =
            BattleManager.instance.field.GetCards((condition) =>
            condition.HaveKeyword(GameEnum.KeywordAbility.EarthSigle), isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        // “y‚Ìˆó‚ğƒvƒ‰ƒX‚·‚é
        if (param[0] > 0)
        {
            // “y‚Ìˆó‚ª‚È‚¢‚È‚ço‚·
            if (IsEmpty(earthSigleCard))
            {

            }
            // ‚ ‚é‚È‚çŠù‘¶‚Ì“y‚Ìˆó‚ğ{
            else
            {
                KeywordAbilityInstance keyword = earthSigleCard[0].GetKeywordAbility(GameEnum.KeywordAbility.EarthSigle);
                keyword.AddParam(param[0]);
            }
        }
        // “y‚Ìˆó‚ğÁ”ï‚·‚é
        else
        {
            if (IsEmpty(earthSigleCard)) return null;
            KeywordAbilityInstance keyword = earthSigleCard[0].GetKeywordAbility(GameEnum.KeywordAbility.EarthSigle);
            if (keyword.param < param[0]) return null;
            keyword.RemoveParam(param[0]);
            // “y‚Ìˆó‚ª‚È‚¢‚È‚ç”j‰ó
            if (keyword.IsNoCount())
                earthSigleCard[0].Destroy();
        }

        return null;
    }
}
