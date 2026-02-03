using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_102 : BaseCardAbility
{
    private const int COMBO_COST = 3;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public　override void Fanfare(bool isOwn)
    {
        // コンボ3 進化
        var targetPlayer = GetPlayer(isOwn);
        if(targetPlayer.leader.comboCount < COMBO_COST) return;
        EvolveEffect evolveEffect = new EvolveEffect(null);
        evolveEffect.ExecuteEffect(sourceData);
    }

    public override void Attack(bool isOwn)
    {
        // 進化後なら自分のリーダー2回復
        if(sourceData.evolveState == CardData.EvolveState.None) return;
        HealEffect healEffect = new HealEffect(new List<int>{2});
        var targetPlayer = GetPlayer(isOwn);
        BaseComponent component = targetPlayer.leader;
        healEffect.ExecuteEffect(component);
    }
}
