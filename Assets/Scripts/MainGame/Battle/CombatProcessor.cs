using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatProcessor
{
    // 攻撃する側のカード
    private CardData attackerCard;

    // 防御する側のカード
    private CardData defenderCard;

    public CombatProcessor(CardData setAttackerCard, CardData setDefenderCard)
    {
        this.attackerCard = setAttackerCard;
        this.defenderCard = setDefenderCard;
    }

    // 通常戦闘
    public void Combat()
    {
        // 攻撃時能力タイミング
        // 交戦時能力タイミング

        int attackerDamage = attackerCard.status.m_attack;
        int defenderDamage = defenderCard.status.m_attack;

        // 受けるダメージ軽減能力タイミング

        defenderCard.DealDamage(attackerDamage);
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
        }

        // 攻撃権限消費
        attackerCard.SetCanAttack(false);

        // 一ターンに～回攻撃出来る能力タイミング


    }

    // 超進化戦闘
    public void SuperEvolveCombat()
    {
        // 攻撃時能力タイミング
        // 交戦時能力タイミング

        int attackerDamage = attackerCard.status.m_attack;
        int defenderDamage = defenderCard.status.m_attack;

        // 受けるダメージ軽減能力タイミング

        defenderCard.DealDamage(attackerDamage);

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

        }

        // 攻撃権限消費
        attackerCard.SetCanAttack(false);

        // 一ターンに～回攻撃出来る能力タイミング
    }


}
