using System;
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
        public int m_defance;    // 体力

        public FollowerStatus(int attack, int defance)
        {
            m_attack = attack;
            m_defance = defance;
        }
    }
    // ダメージの蓄積
    public int damage { get; private set; } = 0;
    // ステータスのバフ/デバフ
    public List<FollowerStatus> addStatus { get; private set; }
    // 基本ステータス
    public FollowerStatus status { get; private set; }
    // 攻撃可能かどうか
    public bool canPlay { get; private set; }
    // 攻撃可能かどうか
    public bool canAttack { get; private set; }
    // アクト可能かどうか
    public bool canAct { get; private set; }
    // カードの種類
    public CardType type { get; private set; }
    // 持っているカードタイプ
    public List<CardTypeDetail> typeDetail { get; private set; }
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
    // カードテキスト
    public string text { get; private set; }
    // カードアビリティ
    public List<CardAbility> ability { get; private set; }
    // 破壊された
    public bool isDestroyed { get; private set; }

    public Func<CardObject> GetObject;

    public void SetGetObjectAction(Func<CardObject> action)
    {
        GetObject = action;
    }

    public CardData(int setID, LeaderClass setClass, CardRarity setRarity, CardType setType, string setName, int setCost, int setAttack, int setDefence)
    {
        id = setID;
        leaderClass = setClass;
        rarity = setRarity;
        type = setType;
        name = setName;
        cost = setCost;
        status = new FollowerStatus(setAttack, setDefence);

        Init();
    }

    public void Init()
    {
        addStatus = new List<FollowerStatus>();
    }

    public void SetType(CardType newType)
    {
        type = newType;
    }

    public void AddTypeDetail(CardTypeDetail newTypeDetail)
    {
        if (typeDetail == null)
        {
            typeDetail = new List<CardTypeDetail>();
        }
        typeDetail.Add(newTypeDetail);
    }
    public void SetCost(int newCost)
    {
        cost = newCost;
    }

    public void SetText(string setText)
    {
        text = setText;
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

        CheckDestroyed();
        GetObject().UpdateText();
    }

    // 破壊されたか
    public bool CheckDestroyed()
    {
        int defance = GetCurrentStatus().m_defance;
        if (defance <= 0)
        {
            Destroy();
        }
        return isDestroyed;
    }

    // 破壊する
    public void Destroy()
    {
        isDestroyed = true;
        // ラストワード発動タイミング

        // フィールドから除去
        BattleManager.instance.field.RemoveCard(this);
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

    public void AddStatus(int attack, int defance)
    {
        FollowerStatus status = new FollowerStatus();
        status.m_attack = attack;
        status.m_defance = defance;
        addStatus.Add(status);
    }

    public void ClearAddStatus()
    {
        addStatus.Clear();
    }

    public void SetCanPlay(bool canPlay)
    {
        this.canPlay = canPlay;
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
            currentStatus.m_defance += addStatus[i].m_defance;
        }
        currentStatus.m_defance -= damage;

        return currentStatus;
    }
}
