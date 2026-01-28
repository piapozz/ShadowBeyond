using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatProcessor
{
    // 攻撃する側のカード
    private CardData attackerCard;

    // 防御する側のカード
    private CardData defenderCard;

    private Leader defenderLeader;

    public CombatProcessor(CardData setAttackerCard, CardData setDefenderCard)
    {
        this.attackerCard = setAttackerCard;
        this.defenderCard = setDefenderCard;
    }

    public CombatProcessor(CardData setAttackerCard, Leader setDefenderLeader)
    {
        this.attackerCard = setAttackerCard;
        this.defenderLeader = setDefenderLeader;
    }

    // 通常戦闘
    public void Combat()
    {
        // 攻撃時能力タイミング
        // 交戦時能力タイミング

        int attackerDamage = attackerCard.GetCurrentStatus().m_attack;
        int defenderDamage = defenderCard.GetCurrentStatus().m_attack;

        // 受けるダメージ軽減能力タイミング

        defenderCard.DealDamage(attackerDamage);
        if (attackerCard.evolveState != CardData.EvolveState.SuperEvolved)
            attackerCard.DealDamage(defenderDamage);

        // ドレイン能力タイミング
        // 必殺能力タイミング

        // 破壊確認
        if (attackerCard.isDestroyed)
        {
            // 攻撃を受けたが破壊されなかった時能力タイミング
        }

        // 破壊確認
        if (defenderCard.isDestroyed)
        {
            // 破壊時能力タイミング

            // 相手リーダーに1ダメージ
            if (attackerCard.evolveState == CardData.EvolveState.SuperEvolved)
            {
                // FIX:リーダーが取得できない
                //defenderLeader.DealDamage(1);
            }
        }

        // 一ターンに～回攻撃出来る能力タイミング


        Debug.Log("Attack : " + attackerCard.name + "Target : " + defenderCard.name);
        Debug.Log("Attacker Damage : " + attackerDamage + "Defender Damage : " + defenderDamage);
    }

    // リーダーへの攻撃
    public void LeaderCombat()
    {
        // 攻撃時能力タイミング
        int attackerDamage = attackerCard.GetCurrentStatus().m_attack;

        // リーダーへの攻撃時能力タイミング

        // リーダーへのダメージ軽減能力タイミング
        // リーダーにダメージ
        defenderLeader.DealDamage(attackerDamage);
        // ドレイン能力タイミング
        // 必殺能力タイミング
        // 一ターンに～回攻撃出来る能力タイミング
        Debug.Log("Attack : " + attackerCard.name + "Target : Leader");
        Debug.Log("Attacker Damage : " + attackerDamage);
    }

}
