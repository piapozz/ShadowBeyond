using System.Collections.Generic;

using static GameEnum;

/// <summary>
/// カードの基底クラス
/// </summary>
public class CardData
{
    // フォロワーのステータス構造体
    public struct FollowerStatus
    {
        public int m_attack;    // 攻撃力
        public int m_health;    // 体力

        public FollowerStatus(int attack, int health)
        {
            m_attack = attack;
            m_health = health;
        }
    }
    // ダメージの蓄積
    public int damage { get; private set; }
    // ステータスのバフ/デバフ
    public List<FollowerStatus> addStatus { get; private set; }
    // 基本ステータス
    public FollowerStatus status { get; private set; }
    // 攻撃可能かどうか
    public bool canAttack { get; private set; }
    // アクト可能かどうか
    public bool canAct { get; private set; }
    // カードの種類
    public CardType cardType { get; private set; }
    // 持っているカードタイプ
    public List<CardTypeDetail> cardTypeDetail { get; private set; }
    // カードid
    public int id { get; private set; }
    // リーダークラス
    public LeaderClass leaderClass { get; private set; }
    // レアリティ
    public CardRarity rarity { get; private set; }
    // カード名
    public string name { get; private set; }
    // カードコスト
    public int cost { get; private set; }
    // カードアビリティ
    public List<CardAbility> ability { get; private set; }

    public void SetType(CardType newType)
    {
        cardType = newType;
    }

    public void AddTypeDetail(CardTypeDetail newTypeDetail)
    {
        if (cardTypeDetail == null)
        {
            cardTypeDetail = new List<CardTypeDetail>();
        }
        cardTypeDetail.Add(newTypeDetail);
    }
    public void SetCost(int newCost)
    {
        cost = newCost;
    }

    public void SetAbility(List<CardAbility> newAbility)
    {
        ability = newAbility;
    }

    public void AddAbility(CardAbility newAbility)
    {
        if (ability == null)
        {
            ability = new List<CardAbility>();
        }
        ability.Add(newAbility);
    }

    public void RemoveAbility(CardAbility removeAbility)
    {
        if (ability == null)
        {
            return;
        }
        ability.Remove(removeAbility);
    }

    public void ClearAbility()
    {
        if (ability == null)
        {
            return;
        }
        ability.Clear();
    }

    public bool CanBeSelected() { return false; }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage"></param>
    public void DealDamage(int damage)
    {
        this.damage += damage;
    }

    /// <summary>
    /// 回復する
    /// </summary>
    /// <param name="heal"></param>
    public void HealDamage(int heal)
    {
        damage -= heal;
        if (damage < 0) damage = 0;
    }

    public void AddStatus(int attack, int health)
    {
        FollowerStatus status = new FollowerStatus();
        status.m_attack = attack;
        status.m_health = health;
        addStatus.Add(status);
    }

    public void ClearAddStatus()
    {
        addStatus.Clear();
    }

    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    public void SetCanAct(bool canAct)
    {
        this.canAct = canAct;
    }

    /// <summary>
    /// 現在のステータスを取得
    /// </summary>
    /// <returns></returns>
    public FollowerStatus GetCurrentStatus()
    {
        FollowerStatus currentStatus = status;
        for (int i = 0, max = addStatus.Count; i < max; i++)
        {
            currentStatus.m_attack += addStatus[i].m_attack;
            currentStatus.m_health += addStatus[i].m_health;
        }
        currentStatus.m_health -= damage;

        return currentStatus;
    }
}
