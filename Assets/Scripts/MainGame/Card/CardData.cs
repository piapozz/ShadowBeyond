using System;
using System.Collections.Generic;

using static GameEnum;

/// <summary>
/// カードの基底クラス
/// </summary>
public class CardData : BaseComponent
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
    // 攻撃権限
    public enum AttackPermission
    {
        NONE = 0,
        CanAttackFollower,
        CanAttackLeader
    }
    // 進化状態
    public enum EvolveState
    {
        None = 0,
        Unevolved,
        Evolved,
        SuperEvolved
    }
    public AttackPermission attackPermission { get; private set; } = AttackPermission.NONE;
    // このターンの攻撃可能回数
    public int remainAttackCount { get; private set; } = 1;
    // 攻撃可能回数
    public int maxAttackCount { get; private set; } = 1;
    // ダメージの蓄積
    public int damage { get; private set; } = 0;
    // ステータスのバフ/デバフ
    public List<FollowerStatus> addStatus { get; private set; }
    // 基本ステータス
    public FollowerStatus status { get; private set; }
    // プレイ可能かどうか
    public bool canPlay { get; private set; }
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
    // デフォルトのコスト
    public int defaultCost { get; private set; }
    // カードテキスト
    public string text { get; private set; }
    public bool isToken { get; private set; }
    public PackType packType { get; private set; }
    // キーワード能力
    public List<KeywordAbilityInstance> keywordAbilities = new();
    // カードアビリティ
    public List<ActiveAbility> activeAbilities { get; private set; }
    // 破壊された
    public bool isDestroyed { get; private set; }
    // 進化状態
    public EvolveState evolveState { get; private set; } = EvolveState.Unevolved;
    public bool isAnyEvolved => evolveState == EvolveState.Evolved || evolveState == EvolveState.SuperEvolved;
    public Func<CardObject> GetObject;

    public void SetGetObjectAction(Func<CardObject> action)
    {
        GetObject = action;
    }

    public CardData(int setID, LeaderClass setClass, CardRarity setRarity, CardType setType, string setName, int setCost, int setAttack, int setDefence, bool setToken)
    {
        id = setID;
        leaderClass = setClass;
        rarity = setRarity;
        type = setType;
        name = setName;
        cost = setCost;
        defaultCost = setCost;
        status = new FollowerStatus(setAttack, setDefence);
        isToken = setToken;

        Init();
    }

    public void Init()
    {
        addStatus = new List<FollowerStatus>();
        activeAbilities = new List<ActiveAbility>();
        keywordAbilities = new List<KeywordAbilityInstance>();
        BaseCardAbility ability = AbilityFactory.GetAbility(id);
        if (ability == null) return;
        ability.Initialize(this);
        activeAbilities.AddRange(ability.activeAbilities);
        keywordAbilities.AddRange(ability.keywordAbilities);
    }

    // ターン開始時処理
    public void OnStartTurn()
    {
        remainAttackCount = maxAttackCount;
        SetAttackPermission(AttackPermission.CanAttackLeader);
    }

    // 攻撃時処理
    public void OnAttack()
    {
        remainAttackCount--;
    }

    // ターン終了時処理
    public void OnEndTurn()
    {
        remainAttackCount = 0;
    }

    public void SetType(CardType setType)
    {
        type = setType;
    }

    public void SetPackType(PackType setPackType)
    {
        packType = setPackType;
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

    public void SetAbility(List<ActiveAbility> newAbility)
    {
        activeAbilities = newAbility;
    }

    public void AddAbility(ActiveAbility newAbility)
    {
        if (activeAbilities == null)
        {
            activeAbilities = new List<ActiveAbility>();
        }
        activeAbilities.Add(newAbility);
    }

    public void RemoveAbility(ActiveAbility removeAbility)
    {
        if (activeAbilities == null)
        {
            return;
        }
        activeAbilities.Remove(removeAbility);
    }

    public void ClearAbility()
    {
        if (activeAbilities == null)
        {
            return;
        }
        activeAbilities.Clear();
    }

    public bool CanBeSelected() { return false; }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(int damage)
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
    public override void HealDamage(int heal)
    {
        damage -= heal;
        if (damage < 0) damage = 0;
        GetObject().UpdateText();
    }

    public void AddStatus(int attack, int defance)
    {
        FollowerStatus status = new FollowerStatus();
        status.m_attack = attack;
        status.m_defance = defance;
        addStatus.Add(status);
        GetObject().UpdateText();
    }

    public void ClearAddStatus()
    {
        addStatus.Clear();
        GetObject().UpdateText();
    }

    public void SetCanPlay(bool canPlay)
    {
        this.canPlay = canPlay;
    }


    public void SetAttackPermission(AttackPermission setAttackPermission)
    {
        // 下位に下がらないようにする
        if (setAttackPermission < attackPermission) return;
        attackPermission = setAttackPermission;
    }

    public void SetCanAct(bool canAct)
    {
        this.canAct = canAct;
    }

    public void SetEvolve()
    {
        SetAttackPermission(AttackPermission.CanAttackFollower);
        evolveState = EvolveState.Evolved;
    }

    public void SetSuperEvolve()
    {
        SetAttackPermission(AttackPermission.CanAttackFollower);
        evolveState = EvolveState.SuperEvolved;
    }

    /// <summary>
    /// 攻撃可能か否か
    /// </summary>
    /// <param name="attackLeader"></param>
    /// <returns></returns>
    public bool CanAttack(bool attackLeader)
    {
        if (remainAttackCount <= 0) return false;

        // リーダーを攻撃する場合
        if (attackLeader)
            return attackPermission == AttackPermission.CanAttackLeader || HaveKeyword(KeywordAbility.Rush);
        // フォロワーを攻撃する場合
        else
        {
            bool result = (attackPermission == AttackPermission.CanAttackLeader) || (attackPermission == AttackPermission.CanAttackFollower);
            if (result) return true;
            result = HaveKeyword(KeywordAbility.Rush) || HaveKeyword(KeywordAbility.Storm);
            return result;
        }
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

    public bool HaveKeyword(KeywordAbility keyword)
    {
        foreach (var keywordInstance in keywordAbilities)
        {
            if (keywordInstance.type == keyword) return true;
        }
        return false;
    }

    public bool HaveDetailType(CardTypeDetail cardTypeDetail)
    {
        foreach (var type in typeDetail)
        {
            if (type == cardTypeDetail) return true;
        }
        return false;
    }
}
