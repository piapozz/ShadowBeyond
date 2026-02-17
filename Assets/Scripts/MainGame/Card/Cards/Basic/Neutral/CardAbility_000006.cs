using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_000006 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
        TargetCondition condition = TargetCondition.Any;
        condition.type.Add(GameEnum.CardType.FOLLOWER);
        selectTarget[(int)TargetTiming.Engage] = new Target(Target.TargetSide.Own, Target.TargetZone.Field, condition, 1);
    }

    public override void Fanfare(bool isOwn)
    {
        // フォロワーを一枚引く
        DrawEffect effect = new DrawEffect(new List<int>{ 1 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck, (card) => { return (card.type == GameEnum.CardType.FOLLOWER); });
    }

    public override void Engage(bool isOwn, List<BaseComponent> selected = null)
    {
        base.Engage(isOwn);
        // これを破壊
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect(sourceData);
        // 自分の場のをフォロワーに突進付与
        if (selected == null) return;

        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int>{(int)GameEnum.KeywordAbility.Rush });
        giveKeywordAbilityEffect.ExecuteEffect(selected[0] as CardData);
    }
}
