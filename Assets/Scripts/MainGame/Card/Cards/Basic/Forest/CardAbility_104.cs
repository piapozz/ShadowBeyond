using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_104 : BaseCardAbility
{
    private const int COMBO_COST = 3;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // 【コンボ_3】相手の場のフォロワー1枚を選ぶ。それに3ダメージ。
        var targetPlayer = GetPlayer(isOwn);
        if (targetPlayer.leader.comboCount < COMBO_COST) return;
        var targetCard = BattleManager.instance.field.GetRandomCard((card) => {return card.type == GameEnum.CardType.FOLLOWER; } , isOwn ? Field.FieldType.OWN : Field.FieldType.OPPONENT);
        if (targetCard == null) return;
        DamageEffect damageEffect = new DamageEffect(new List<int> { 3 });
        damageEffect.ExecuteEffect(targetCard);
    }
}
