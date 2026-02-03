using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_6 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Fanfare(bool isOwn)
    {
        // フォロワーを一枚引く
        DrawEffect effect = new DrawEffect(new List<int>{ 1 });
        var targetDeck = GetPlayer(isOwn).deck;
        effect.ExecuteEffect(targetDeck, (card) => { return (card.type == GameEnum.CardType.FOLLOWER); });
    }

    public override void Engage(bool isOwn)
    {
        // これを破壊
        DestroyEffect destroyEffect = new DestroyEffect(null);
        destroyEffect.ExecuteEffect(sourceData);
        // 自分の場のをフォロワーを1体選ぶ。突進
        var field = BattleManager.instance.field;
        var targetCard = field.GetRandomCard((card) => { return card.type == GameEnum.CardType.FOLLOWER; }, ! isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        GiveKeywordAbilityEffect giveKeywordAbilityEffect = new GiveKeywordAbilityEffect(new List<int>{(int)GameEnum.KeywordAbility.Rush });
        giveKeywordAbilityEffect.ExecuteEffect(targetCard);
    }
}
