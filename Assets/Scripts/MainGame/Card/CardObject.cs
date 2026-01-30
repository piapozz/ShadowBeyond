using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

/// <summary>
/// カードオブジェクトにアタッチするクラス
/// </summary>
public class CardObject : BaseFieldObject
{
    [SerializeField]
    private List<GameObject> cardPrefab = null;
    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private List<Material> cardMaterial = null;
    [SerializeField]
    private List<GameObject> effectObject = null;

    public enum CardObjectType
    {
        INVALID = -1,
        DEFAULT_FOLLOWER,
        HAND_FOLLOWER,
        HAND_SPELL,
        HAND_AMULET,
        FIELD_FOLLOWER,
        EVOLVE_FOLLOWER,
        SUPWER_EVOLVE_FOLLOWER,
        FIELD_AMULET,
        MAX
    }

    public enum CardState
    {
        INVALID = -1,
        UNUSE,
        HAND,
        FIELD,
        REDRAW,
        MAX
    }

    public CardState currentState { get; private set; } = CardState.HAND;

    // カードクラスの参照
    public CardData cardData { get; private set; } = null;
    private GameObject[] cardObject = new GameObject[(int)CardState.MAX];
    public List<GameObject> GetCardObject() { return new List<GameObject>(cardObject); }

    // カードをドラッグした時の高さオフセット
    private const float OFFSET_Y = 1.0f;

    /// <summary>
    /// カードクラスを渡す
    /// </summary>
    public void SetCardData(CardData setCard)
    {
        cardData = setCard;
        cardData.SetGetObjectAction(() => { return this; });
        SetCardLook();
    }

    private void OnMouseDown()
    {
        if (!isLocal) return;
        switch (currentState)
        {
            case CardState.HAND:
                break;
            case CardState.FIELD:
                // フォロワー以外は攻撃できない
                if (cardData.type != GameEnum.CardType.FOLLOWER) return;
                // 攻撃可否判定
                if (!cardData.CanAttack(false)) return;
                UIManager.instance.SetUIState(UIManager.UIState.ATTACK);
                // カードを持ち上げる
                GetPickupSequence(true).Play();
                // 攻撃の線を出す
                UIManager.instance.SetLineRenderer(lineRenderer, transform);
                lineRenderer.enabled = true;
                break;
            case CardState.REDRAW:
                break;
            default: break;
        }
    }

    private void OnMouseDrag()
    {
        if (!isLocal) return;
        switch (currentState)
        {
            case CardState.HAND:
                // プレイ可能でないなら返す
                if (!cardData.canPlay) return;
                // カードの位置更新
                Vector3 position = UIManager.instance.GetMouseWorldPosition(transform);
                position.y = OFFSET_Y;
                transform.position = position;
                break;
            case CardState.FIELD:
                // フォロワー以外は攻撃できない
                if (cardData.type != GameEnum.CardType.FOLLOWER) return;
                // 攻撃可否判定
                if (!cardData.CanAttack(false)) return;
                // 攻撃の線を出す
                UIManager.instance.SetLineRenderer(lineRenderer, transform);
                break;
            case CardState.REDRAW:
                // カードの位置更新
                position = UIManager.instance.GetMouseWorldPosition(transform);
                position.y = OFFSET_Y;
                transform.position = position;
                break;
            default: break;
        }
    }

    private async void OnMouseUp()
    {
        if (!isLocal) return;
        switch (currentState)
        {
            case CardState.HAND:
                // プレイ可能でないなら返す
                if (!cardData.canPlay) return;
                // オブジェクトをUIにセット
                UIManager.instance.DropCard(this);
                break;
            case CardState.FIELD:
                // フォロワー以外は攻撃できない
                if (cardData.type != GameEnum.CardType.FOLLOWER) return;
                // 攻撃可否判定
                if (!cardData.CanAttack(false)) return;
                lineRenderer.enabled = false;
                // 攻撃処理
                bool attackResult = Attack();
                UIManager.instance.SetUIState(UIManager.UIState.DEFAULT);
                if (!attackResult)
                    GetPickupSequence(false).Play();
                break;
            case CardState.REDRAW:
                // オブジェクトをUIにセット
                await UIManager.instance.RedrawDropCard(this);
                break;
            default: break;
        }
    }

    private void OnMouseEnter()
    {
        // 攻撃時に相手のフィールドのカードを持ち上げる
        if (isLocal || UIManager.instance.state != UIManager.UIState.ATTACK) return;
        if (currentState != CardState.FIELD || cardData.type != GameEnum.CardType.FOLLOWER) return;
        GetPickupSequence(true).Play();
    }

    private void OnMouseExit()
    {
        if (isLocal || UIManager.instance.state != UIManager.UIState.ATTACK) return;
        if (currentState != CardState.FIELD || cardData.type != GameEnum.CardType.FOLLOWER) return;
        GetPickupSequence(false).Play();
    }

    public Sequence GetPickupSequence(bool isPick)
    {
        float offsetY = isPick ? OFFSET_Y : 0.0f;
        Vector3 scale = isPick ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;

        Sequence pickupSeq = DOTween.Sequence();
        pickupSeq.Append(transform.DOMoveY(offsetY, 0.2f))
            .Join(transform.DOScale(scale, 0.1f));
        return pickupSeq;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <returns>攻撃成功可否</returns>
    private bool Attack()
    {
        // マウスの座標からオブジェクトを取得
        BaseFieldObject target = UIManager.instance.GetFieldObject(Input.mousePosition);
        if (target == null) return false;
        // 自分自身は攻撃できない
        if (target.isLocal) return false;
        CardObject targetCard = target as CardObject;
        if (targetCard != null)
        {
            return AttackFollower(targetCard);
        }

        LeaderObject targetLeader = target as LeaderObject;
        if (targetLeader != null)
        {
            return AttackLeader(targetLeader);
        }

        return false;
    }

    /// <summary>
    /// フォロワーへの攻撃
    /// </summary>
    /// <param name="targetCard"></param>
    private bool AttackFollower(CardObject targetCard)
    {
        CardData defenceCard = targetCard.cardData;
        // 攻撃可能オブジェクトか判定(フィールドに出ている敵フォロワーか敵リーダー)
        if (targetCard.currentState != CardState.FIELD || defenceCard.type != GameEnum.CardType.FOLLOWER) return false;
        // 攻撃可否判定
        if (!cardData.CanAttack(false)) return false;
        // 潜伏か威圧持ちなら攻撃できない
        if (defenceCard.HaveKeyword(GameEnum.KeywordAbility.Ambush) ||
            defenceCard.HaveKeyword(GameEnum.KeywordAbility.Intimidate))
            return false;
        // 守護を持っていなく、守護持ちフォロワーがいるなら攻撃できない
        if (!defenceCard.HaveKeyword(GameEnum.KeywordAbility.Ward) && BattleManager.instance.IsWardOpponentField())
            return false;
        cardData.OnAttack();
        AttackFollower(targetCard);

        // 情報を送信
        int sourceIndex = UIManager.instance.GetOwnFieldIndex(this);
        int targetIndex = UIManager.instance.GetOpponentFieldIndex(targetCard);
        BattleManager.instance.SendInputData(GameEnum.InputType.ATTACK_FOLLOWER, new int[2] { sourceIndex, targetIndex });
        // 攻撃処理を依頼
        BattleManager.instance.CardCombat(cardData, targetCard.cardData);
        // 挙動
        UIManager.instance.SetAttackFollowerSequence(this, targetCard);
        return true;
    }

    /// <summary>
    /// リーダーへの攻撃
    /// </summary>
    /// <param name="leaderCard"></param>
    private bool AttackLeader(LeaderObject leaderCard)
    {
        // 攻撃可否判定
        if (!cardData.CanAttack(true)) return false;
        // 守護を持っているフォロワーがいるなら攻撃できない
        if (BattleManager.instance.IsWardOpponentField()) return false;
        cardData.OnAttack();

        // 情報を送信
        int sourceIndex = UIManager.instance.GetOwnFieldIndex(this);
        BattleManager.instance.SendInputData(GameEnum.InputType.ATTACK_LEADER, new int[1] { sourceIndex });
        // 攻撃処理を依頼
        BattleManager.instance.LeaderCombat(cardData, leaderCard.leader);
        // 挙動
        UIManager.instance.SetAttackLeaderSequence(this);
        return true;
    }

    public Sequence GetAttackSequence()
    {
        Sequence attack = DOTween.Sequence();
        attack.Append(GetFlipCard(0.1f));
        return attack;
    }

    public Sequence GetCounterAttackSequence()
    {
        Sequence counterAttack = DOTween.Sequence();
        counterAttack.Append(GetDefenceSequence());
        counterAttack.Append(GetFlipCard(0.1f));
        return counterAttack;
    }

    public Sequence GetDefenceSequence()
    {
        float direction = isLocal ? -1.0f : 1.0f;

        Sequence defence = DOTween.Sequence();
        defence.Append(transform.DOMoveZ(direction * 0.5f, 0.1f))
            .SetLoops(2, LoopType.Yoyo);
        return defence;
    }

    /// <summary>
    /// カードの状態の設定
    /// </summary>
    /// <param name="state"></param>
    public void SetCardState(CardState state)
    {
        currentState = state;
        // オブジェクトの切り替え
        switch (currentState)
        {
            case CardState.HAND:
                gameObject.SetActive(true);
                cardObject[(int)CardState.HAND].SetActive(true);
                cardObject[(int)CardState.FIELD].SetActive(false);
                cardObject[(int)CardState.REDRAW].SetActive(false);
                DisableAllCardEffect();
                break;
            case CardState.FIELD:
                gameObject.SetActive(true);
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(true);
                cardObject[(int)CardState.REDRAW].SetActive(false);
                break;
            case CardState.UNUSE:
                gameObject.SetActive(false);
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(false);
                cardObject[(int)CardState.REDRAW].SetActive(false);
                DisableAllCardEffect();
                break;
            case CardState.REDRAW:
                gameObject.SetActive(true);
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(false);
                cardObject[(int)CardState.REDRAW].SetActive(true);
                DisableAllCardEffect();
                break;
            default: break;
        }
    }

    /// <summary>
    /// カードの見た目の適用
    /// </summary>
    public void SetCardLook()
    {
        foreach (GameObject obj in cardObject)
        {
            if (obj != null)
                Destroy(obj);
        }

        // オブジェクト設定
        switch (cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
                cardObject[(int)CardState.HAND] = Instantiate(cardPrefab[(int)CardObjectType.HAND_FOLLOWER], this.transform);
                cardObject[(int)CardState.FIELD] = Instantiate(cardPrefab[(int)CardObjectType.FIELD_FOLLOWER], this.transform);
                cardObject[(int)CardState.REDRAW] = Instantiate(cardPrefab[(int)CardObjectType.DEFAULT_FOLLOWER], this.transform);
                break;
            case GameEnum.CardType.SPELL:
                cardObject[(int)CardState.HAND] = Instantiate(cardPrefab[(int)CardObjectType.HAND_SPELL], this.transform);
                cardObject[(int)CardState.FIELD] = Instantiate(cardPrefab[(int)CardObjectType.HAND_SPELL], this.transform);
                cardObject[(int)CardState.REDRAW] = Instantiate(cardPrefab[(int)CardObjectType.HAND_SPELL], this.transform);
                break;
            case GameEnum.CardType.AMULET:
                cardObject[(int)CardState.HAND] = Instantiate(cardPrefab[(int)CardObjectType.HAND_AMULET], this.transform);
                cardObject[(int)CardState.FIELD] = Instantiate(cardPrefab[(int)CardObjectType.FIELD_AMULET], this.transform);
                cardObject[(int)CardState.REDRAW] = Instantiate(cardPrefab[(int)CardObjectType.HAND_AMULET], this.transform);
                break;
            default: break;
        }
        // 手札オブジェクト設定
        // テキスト設定
        CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
        if (handLook == null) return;
        handLook.SetCardText(cardData);
        // マテリアル設定
        handLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);

        // フィールドオブジェクト設定
        // テキスト設定
        CardLook fieldLook = cardObject[(int)CardState.FIELD].GetComponent<CardLook>();
        if (handLook == null) return;
        fieldLook.SetCardText(cardData);
        // マテリアル設定
        fieldLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);

        CardLook redrawLook = cardObject[(int)CardState.REDRAW].GetComponent<CardLook>();
        if (redrawLook == null) return;
        redrawLook.SetCardText(cardData);
        // マテリアル設定
        redrawLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);

        SetCardState(CardState.UNUSE);
    }

    public void UpdateText()
    {
        // 手札オブジェクト設定
        // テキスト設定
        CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
        if (handLook == null) return;
        handLook.SetCardText(cardData);

        // フィールドオブジェクト設定
        // テキスト設定
        CardLook fieldLook = cardObject[(int)CardState.FIELD].GetComponent<CardLook>();
        if (handLook == null) return;
        fieldLook.SetCardText(cardData);

        CardLook redrawLook = cardObject[(int)CardState.REDRAW].GetComponent<CardLook>();
        if (redrawLook == null) return;
        redrawLook.SetCardText(cardData);
    }

    public Sequence GetFlipCard(float flipSpeed)
    {
        Sequence flipSequence = DOTween.Sequence();
        flipSequence.Append(transform.DORotate(new Vector3(0, 0, 360), flipSpeed, RotateMode.LocalAxisAdd));
        return flipSequence;
    }

    /// <summary>
    /// 自分のドロー
    /// </summary>
    /// <param name="deckRoot"></param>
    /// <param name="drawRoot"></param>
    /// <param name="cardRoot"></param>
    /// <returns></returns>
    public Sequence DrawOwnCard(Transform deckRoot, Transform drawRoot, Transform cardRoot, Transform handRoot, float sec)
    {
        // ドロールートまでの挙動
        Sequence drawSeq = DOTween.Sequence();
        drawSeq.AppendCallback(() => transform.position = deckRoot.position)
            .JoinCallback(() => transform.rotation = deckRoot.rotation)
            .JoinCallback(() => gameObject.SetActive(true))
            .Join(transform.DOMove(drawRoot.position, sec))
            .Join(transform.DORotate(drawRoot.localEulerAngles, sec));
        // 手札ルートまでの挙動
        drawSeq.Append(transform.DORotate(handRoot.localEulerAngles, sec))
            .Join(transform.DOMove(cardRoot.position, sec))
            .JoinCallback(() => transform.SetParent(handRoot));
        return drawSeq;
    }

    /// <summary>
    /// 相手のドロー
    /// </summary>
    /// <param name="deckRoot"></param>
    /// <param name="cardRoot"></param>
    /// <returns></returns>
    public Sequence DrawOpponentCard(Transform deckRoot, Transform cardRoot, Transform handRoot)
    {
        // 表を非表示
        CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
        if (handLook == null) return null;
        handLook.SetCardFrontActive(false);
        Sequence drawSeq = DOTween.Sequence();
        // 手札ルートまでの挙動
        drawSeq.AppendCallback(() => transform.position = deckRoot.position)
            .JoinCallback(() => transform.rotation = deckRoot.rotation)
            .JoinCallback(() => gameObject.SetActive(true))
            .Join(transform.DORotate(handRoot.localEulerAngles, 0.5f))
            .Join(transform.DOMove(cardRoot.position, 0.5f))
            .JoinCallback(() => transform.SetParent(handRoot));
        return drawSeq;
    }

    /// <summary>
    /// 自分のデッキ戻し
    /// </summary>
    public Sequence ReturnOwnCard(Transform deckRoot, Transform returnRoot, Transform handRoot)
    {
        // 戻しルートまでの挙動
        Sequence returnSeq = DOTween.Sequence();
        returnSeq.AppendCallback(() => transform.DORotate(deckRoot.localEulerAngles, 0.5f))
            .JoinCallback(() => transform.SetParent(null))
            .Join(transform.DOMove(returnRoot.position, 0.5f));
        // デッキルートまでの挙動
        returnSeq.Append(transform.DOMove(deckRoot.position, 0.5f))
            .Join(transform.DORotate(deckRoot.localEulerAngles, 0.5f))
            .AppendCallback(() => SetCardState(CardState.UNUSE));
        return returnSeq;
    }

    /// <summary>
    /// 相手のデッキ戻し
    /// </summary>
    public Sequence ReturnOpponentCard(Transform deckRoot, Transform handRoot)
    {
        // 表を非表示
        CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
        if (handLook == null) return null;
        handLook.SetCardFrontActive(false);
        Sequence returnSeq = DOTween.Sequence();
        // デッキルートまでの挙動
        returnSeq.AppendCallback(() => transform.rotation = deckRoot.rotation)
            .JoinCallback(() => gameObject.SetActive(true))
            .Join(transform.DORotate(deckRoot.localEulerAngles, 0.5f))
            .Join(transform.DOMove(deckRoot.position, 0.5f))
            .JoinCallback(() => transform.SetParent(null))
            .AppendCallback(() => SetCardState(CardState.UNUSE));
        return returnSeq;
    }

    /// <summary>
    /// プレイ時のシークエンス取得
    /// </summary>
    /// <param name="isOwn"></param>
    /// <param name="playCardSlot"></param>
    /// <param name="fieldRoot"></param>
    /// <returns></returns>
    public Sequence PlayFieldSequence(bool isOwn, Transform playCardSlot, Transform fieldRoot)
    {
        // 相手のカードなら表を表示
        if (!isOwn)
        {
            CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
            if (handLook == null) return null;
            handLook.SetCardFrontActive(true);
        }

        // 表示するべきエフェクトを取得
        List<GameObject> activeEffect = GetCardEffectList();
        DisableAllCardEffect();
        Sequence toFieldSequence = DOTween.Sequence();
        toFieldSequence.Append(transform.DOMove(playCardSlot.position, 0.3f))
            .Join(transform.DOScale(playCardSlot.localScale, 0.3f))
            .JoinCallback(() => transform.SetParent(fieldRoot))
            .AppendCallback(() => PlayEffect(EffectManager.EffectType.OnField, 1.0f))
            .AppendCallback(() => PlayCard(isOwn))
            .AppendCallback(() =>
            {
                 // エフェクトを有効化
                 foreach (GameObject effect in activeEffect)
                 {
                     effect.SetActive(true);
                 }
            });

        // 手札のプレイ可否を設定
        Hand currentHand = BattleManager.instance.GetCurrentPlayer().hand;
        currentHand.PlayCardToField(cardData);
        return toFieldSequence;
    }

    public void PlaySpellCard(bool isOwn)
    {
        // 相手のカードなら表を表示
        if (!isOwn)
        {
            CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
            if (handLook == null) return;
            handLook.SetCardFrontActive(true);
        }
        // プレイ
        PlayCard(isOwn);
        // 手札のプレイ可否を設定
        Hand currentHand = BattleManager.instance.GetCurrentPlayer().hand;
        currentHand.PlayCardToField(cardData);
    }

    public void PlayCard(bool isOwn)
    {
        if (cardData == null) return;
        Leader leader = BattleManager.instance.GetCurrentPlayer().leader;
        // スペル、ファンファーレ、エンハンスはここで発動
        int cardCost = cardData.GetPlayableCost(leader.currentPlayPoint);
        if (cardCost < 0) return;
        bool isEnhance = false;
        if (cardCost != cardData.cost) isEnhance = true;
        cardData.OnPlay(isOwn, isEnhance);
        // PP消費
        leader.SetCurrentPlayPoint(leader.currentPlayPoint - cardCost);
    }

    public Sequence GetPlaySequence(Transform playCardRoot)
    {
        // プレイ時のアニメーション
        Sequence playSequence = DOTween.Sequence();
        playSequence.AppendCallback(() => transform.eulerAngles = new Vector3(0, 0, 180))
            .Join(transform.DOMove(playCardRoot.position, 0.3f))
            .Join(transform.DORotate(new Vector3(0, 0, 180), 0.3f, RotateMode.LocalAxisAdd))
            .Join(transform.DOScale(playCardRoot.localScale, 0.3f))
            .AppendCallback(() => AudioManager.instance.PlaySE(AudioManager.SEType.CARD_PLAY))
            .AppendInterval(0.3f);
        // カードタイプによって挙動を分ける
        switch (cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
            case GameEnum.CardType.AMULET:
                playSequence.AppendCallback(() => SetCardState(CardState.FIELD));
                break;
            case GameEnum.CardType.SPELL:
                playSequence.AppendCallback(() => SetCardState(CardState.UNUSE))
                    .JoinCallback(() => transform.localScale = Vector3.one);
                break;
            default: break;
        }
        return playSequence;
    }

    public void EvolveFollower()
    {
        cardData.SetEvolve();
        cardData.AddStatus(2, 2);
        // モデルを切り替える
        cardObject[(int)CardState.FIELD].SetActive(false);
        cardObject[(int)CardState.FIELD] = Instantiate(cardPrefab[(int)CardObjectType.EVOLVE_FOLLOWER], this.transform);
        // テキスト設定
        CardLook fieldLook = cardObject[(int)CardState.FIELD].GetComponent<CardLook>();
        if (fieldLook == null) return;
        fieldLook.SetCardText(cardData);
        cardObject[(int)CardState.FIELD].SetActive(true);
    }

    public void SuperEvolveFollower()
    {
        cardData.SetSuperEvolve();
        cardData.AddStatus(3, 3);
        // モデルを切り替える
        cardObject[(int)CardState.FIELD].SetActive(false);
        cardObject[(int)CardState.FIELD] = Instantiate(cardPrefab[(int)CardObjectType.SUPWER_EVOLVE_FOLLOWER], this.transform);
        // テキスト設定
        CardLook fieldLook = cardObject[(int)CardState.FIELD].GetComponent<CardLook>();
        if (fieldLook == null) return;
        fieldLook.SetCardText(cardData);
        cardObject[(int)CardState.FIELD].SetActive(true);
    }

    public void CheckDestroyCard()
    {
        if (!cardData.isDestroyed) return;

        // 破壊エフェクト
        PlayEffect(EffectManager.EffectType.OnDestroy, 1.0f);
        // 音再生
        AudioManager.instance.PlaySE(AudioManager.SEType.CARD_DESTROY);

        // オブジェクト非表示
        SetCardState(CardState.UNUSE);

        // フィールドから除外
        UIManager.instance.RemoveFieldCard(this);
    }

    public void PlayEffect(EffectManager.EffectType type, float sec)
    {
        EffectManager.Instance.PlayEffect(type, transform.position + new Vector3(0, 1.0f, 0), sec);
    }

    public List<GameObject> GetCardEffectList()
    {
        List<GameObject> activeEffect = new List<GameObject>();
        // ついている能力から表示するべきエフェクトを判定
        for (int i = 0; i < (int)GameEnum.KeywordAbility.MAX; i++)
        {
            if(cardData.HaveKeyword((GameEnum.KeywordAbility)i))
            {
                activeEffect.Add(effectObject[i]);
            }
        }

        return activeEffect;
    }

    // カードエフェクトを全て非表示にする
    public void DisableAllCardEffect()
    {
        foreach (GameObject effect in effectObject)
        {
            effect.SetActive(false);
        }
    }
}
