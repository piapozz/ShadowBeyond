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
    public List<FollowerStatus> addStatus { get; private set; } = new List<FollowerStatus>();
    // 基本ステータス
    public FollowerStatus status { get; private set; }
    // プレイ可能かどうか
    public bool canPlay { get; private set; }
    // アクト可能かどうか
    public bool canAct { get; private set; } = true;
    // 融合可能かどうか
    public bool canFusion { get; private set; } = true;
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
    public string crestText { get; private set; }
    public bool isToken { get; private set; }
    public PackType packType { get; private set; }
    // 能力クラス
    public BaseCardAbility ability { get; private set; }
    // 破壊された
    public bool isDestroyed { get; private set; }
    // 進化状態
    public EvolveState evolveState { get; private set; } = EvolveState.Unevolved;
    public bool isAnyEvolved => evolveState == EvolveState.Evolved || evolveState == EvolveState.SuperEvolved;

    public CardObject GetCardObject()
    {
        return GetObject() as CardObject;
    }

    public CardData(int setID, LeaderClass setClass, CardRarity setRarity, CardType setType, string setName, int setCost, int setAttack, int setDefence, bool setToken, int[] setTrait)
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
        SetTrait(setTrait);
    }

    public void Init()
    {
        ability = AbilityFactory.GetAbility(id);
        if (ability == null) return;
        ability.Initialize(this);
    }


    private void SetTrait(int[] setTrait)
    {
        for (int i = 0, max = setTrait.Length; i < max; i++)
        {
            CardTypeDetail trait = (CardTypeDetail)setTrait[i];
            AddTypeDetail(trait);
        }
    }

    public void OnPlay(bool isOwn, bool isEnhance)
    {
        if (ability == null) return;
        if (isEnhance) ability.Enhance(isOwn);
        else ability.Fanfare(isOwn);
        GetCardObject().SetAttackPermissionLook();
    }

    // ターン開始時処理
    public void OnStartTurn()
    {
        remainAttackCount = maxAttackCount;
        canAct = true;
        canFusion = true;
        SetAttackPermission(AttackPermission.CanAttackLeader);
        GetCardObject().SetAttackPermissionLook();
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
        GetCardObject().SetAttackPermissionLook();
    }

    /// <summary>
    /// 場に出たとき処理
    /// </summary>
    /// <param name="isOwn"></param>
    public void OnEnterField(bool isOwn)
    {
        AbilityManager.TriggerTiming timing = isOwn ? AbilityManager.TriggerTiming.OwnEnterField : AbilityManager.TriggerTiming.OpponentEnterField;
        bool isOwnTurn = BattleManager.instance.IsOwnTurn();
        AbilityManager.Trigger(timing, isOwnTurn, this);

        if (ability == null) return;
        int abilityCount = ability.activeAbilities.Count;
        if (abilityCount <= 0) return;
        // アビリティを登録
        for (int i = 0; i < abilityCount; i++)
        {
            AbilityManager.SubscribeAbility(ability.activeAbilities[i], isOwn);
        }
    }

    /// <summary>
    /// 場を離れたとき処理
    /// </summary>
    /// <param name="isOwn"></param>
    public void OnLeaveField(bool isOwn)
    {
        if (ability == null) return;
        int abilityCount = ability.activeAbilities.Count;
        if (abilityCount <= 0) return;
        // アビリティを登録解除
        for (int i = 0; i < abilityCount; i++)
        {
            AbilityManager.UnsubscribeAbility(ability.activeAbilities[i]);
        }
    }

    public void OnAct()
    {
        canAct = false;
    }

    public void OnFusion()
    {
        canFusion = false;
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

    public void SetText(string setText, string setCrestText = null)
    {
        text = setText;
        crestText = setCrestText;
    }

    public void SetAbility(List<ActiveAbility> newAbility)
    {
        ability.activeAbilities = newAbility;
    }

    public void AddAbility(ActiveAbility newAbility)
    {
        if (ability.activeAbilities == null)
        {
            ability.activeAbilities = new List<ActiveAbility>();
        }
        ability.activeAbilities.Add(newAbility);
    }

    public void RemoveAbility(ActiveAbility removeAbility, CardData sourceCard = null)
    {
        if (ability.activeAbilities == null)
        {
            return;
        }
        // ソースカードがないなら指定キーワード全削除
        if (sourceCard == null)
        {
            ability.activeAbilities.RemoveAll(activeAbility => activeAbility == removeAbility);
        }
        // ソースがあるならソースカード由来のキーワードのみ削除
        else
        {
            ability.activeAbilities.RemoveAll(activeAbility => activeAbility == removeAbility && activeAbility.sourceCard == sourceCard);
        }
    }

    public void ClearAllAbility()
    {
        if (ability.activeAbilities == null)
        {
            return;
        }
        ability.activeAbilities.Clear();
        if (ability.keywordAbilities == null)
        {
            return;
        }
        ability.keywordAbilities.Clear();
    }

    public void AddKeyword(KeywordAbilityInstance addKeyword)
    {
        if (ability.keywordAbilities == null)
        {
            ability.keywordAbilities = new List<KeywordAbilityInstance>();
        }
        ability.keywordAbilities.Add(addKeyword);
        GetCardObject().SetAttackPermissionLook();
    }

    public void RemoveKeyword(KeywordAbility removeKeyword, CardData sourceCard = null)
    {
        if (ability.keywordAbilities == null)
        {
            return;
        }
        // ソースカードがないなら指定キーワード全削除
        if (sourceCard == null)
        {
            ability.keywordAbilities.RemoveAll(keyword => keyword.type == removeKeyword);
        }
        // ソースがあるならソースカード由来のキーワードのみ削除
        else
        {
            ability.keywordAbilities.RemoveAll(keyword => keyword.type == removeKeyword && keyword.source == sourceCard);
        }
        GetCardObject().SetAttackPermissionLook();
    }

    public bool CanBeSelected() { return false; }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(int damage)
    {
        this.damage += damage;

        GetCardObject().UpdateText();
        GetCardObject().PlayEffect(EffectManager.EffectType.AttackDamage, 1.0f);
        AudioManager.instance.PlaySE(AudioManager.SEType.DAMAGE);
        CheckDestroyed();
    }

    // 破壊されたか
    public void CheckDestroyed()
    {
        int defance = GetCurrentStatus().m_defance;
        if (defance <= 0)
            Destroy();
    }
    // 破壊する
    public void Destroy()
    {
        isDestroyed = true;
        bool isOwn = GetObject().isLocal;
        // フィールドから除去
        BattleManager.instance.field.RemoveCard(this, isOwn);
        // ラストワード発動タイミング
        if (ability == null) return;
        ability.LastWord(isOwn);
        // リーダーに記録
        Leader leader = BattleManager.instance.GetPlayer(isOwn ? 0 : 1).leader;
        leader.AddHistory(BattleStatType.DestroyedFollowers, this);
        if (!isOwn) return;
        UIManager.instance.AddInfo(this);
    }

    // 消滅
    public void Banish()
    {
        isDestroyed = true;
        bool isOwn = GetObject().isLocal;
        // フィールドから除去
        BattleManager.instance.field.RemoveCard(this, isOwn);
    }

    /// <summary>
    /// 回復する
    /// </summary>
    /// <param name="heal"></param>
    public override void HealDamage(int heal)
    {
        damage -= heal;
        if (damage < 0) damage = 0;
        GetCardObject().UpdateText();
        GetCardObject().PlayEffect(EffectManager.EffectType.Heal, 1.0f);
        AudioManager.instance.PlaySE(AudioManager.SEType.HEAL);
    }

    public void AddStatus(int attack, int defance)
    {
        FollowerStatus status = new FollowerStatus();
        status.m_attack = attack;
        status.m_defance = defance;
        addStatus.Add(status);
        GetCardObject().UpdateText();
        GetCardObject().PlayEffect(EffectManager.EffectType.StatusUp, 1.0f);
    }

    public void ClearAddStatus()
    {
        addStatus.Clear();
        GetCardObject().UpdateText();
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
        AddStatus(2, 2);
        SetAttackPermission(AttackPermission.CanAttackFollower);
        evolveState = EvolveState.Evolved;
    }

    public void SetSuperEvolve()
    {
        AddStatus(3, 3);
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
        AttackPermission currentAttacPermission = GetAttackPermission();
        if (currentAttacPermission == AttackPermission.NONE) return false;
        // リーダーを攻撃する場合
        if (attackLeader)
            return currentAttacPermission == AttackPermission.CanAttackLeader;
        // フォロワーを攻撃する場合
        else
            return currentAttacPermission == AttackPermission.CanAttackLeader || currentAttacPermission == AttackPermission.CanAttackFollower;
    }

    public AttackPermission GetAttackPermission()
    {
        if (remainAttackCount <= 0) return AttackPermission.NONE;

        if (attackPermission == AttackPermission.CanAttackLeader || HaveKeyword(KeywordAbility.Storm)) return AttackPermission.CanAttackLeader;

        if ((attackPermission == AttackPermission.CanAttackFollower) || HaveKeyword(KeywordAbility.Rush)) return AttackPermission.CanAttackFollower;

        return AttackPermission.NONE;
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
        if (ability == null) return false;
        foreach (var keywordInstance in ability.keywordAbilities)
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

    /// <summary>
    /// エンハンスを含むプレイ可能コストを取得
    /// </summary>
    /// <param name="currentPP"></param>
    /// <returns></returns>
    public int GetPlayableCost(int currentPP)
    {
        if (currentPP < cost) return -1;
        // エンハンス
        if (!HaveKeyword(GameEnum.KeywordAbility.Enhance)) return cost;
        KeywordAbilityInstance enhance = null;
        ability.keywordAbilities.ForEach(keywordAbility =>
        {
            if (keywordAbility.type == GameEnum.KeywordAbility.Enhance)
            {
                enhance = keywordAbility;
            }
        });
        int enhanceCost = enhance.param;
        if (enhanceCost > currentPP) return cost;
        return enhanceCost;
    }

    public KeywordAbilityInstance GetKeywordAbility(KeywordAbility keyword)
    {
        if (ability == null) return null;
        foreach (var keywordInstance in ability.keywordAbilities)
        {
            if (keywordInstance.type == keyword) return keywordInstance;
        }
        return null;
    }
}
