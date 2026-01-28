using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class CardAbility_Test : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush));
        activeAbilities.Add(new ActiveAbility(
            AbilityManager.TriggerTiming.Play,
            new Target(),
            new BuffEffect(new List<int> { 1, 2 }),
            null,
            ActiveAbility.Zone.Field,
            setCard
            ));
    }

    public override void Fanfare(bool isOwn)
    {
        Target target = new Target(Target.TargetSide.Own, Target.TargetZone.Field, TargetCondition.Any,
            false, false);
        // ƒ^[ƒQƒbƒg‚ğBattleManager‚É“n‚µ‚ÄList‚Åó‚¯æ‚é
        List<BaseComponent> components = BattleManager.instance.GetTargetCard(target, isOwn);
        // effect‚É“n‚·
        BaseEffect effect = new BuffEffect(new List<int> { 1, 2 });
        effect.ExecuteEffect(components);
    }
}
