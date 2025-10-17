using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

using DG.Tweening;
using static CommonModule;

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

    public enum CardObjectType
    {
        INVALID = -1,
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
        MAX
    }

    private CardState currentState = CardState.HAND;

    // カードクラスの参照
    public CardData cardData { get; private set; } = null;
    private GameObject[] cardObject = new GameObject[(int)CardState.MAX];
    private Camera mainCamera = null;

    // カードをドラッグした時の高さオフセット
    private const float OFFSET_Y = 1.0f;
    // 攻撃線の制御点数
    private const int LINE_CONTROL_POINT_NUM = 20;
    // 攻撃線の高さ
    private const float LINE_HEIGHT = 5.0f;
    // 攻撃線のずらし幅
    private const float LINE_OFFSET = 2.0f;

    public void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.enabled = false;
    }

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
        if (!isLocal || !BattleManager.instance.IsOwnTurn()) return;
        switch (currentState)
        {
            case CardState.HAND:
                break;
            case CardState.FIELD:
                // 攻撃の線を出す
                lineRenderer.enabled = true;
                break;
            default: break;
        }
    }

    private void OnMouseDrag()
    {
        if (!isLocal || !BattleManager.instance.IsOwnTurn()) return;
        switch (currentState)
        {
            case CardState.HAND:
                // カードの位置更新
                Vector3 position = GetMouseWorldPosition(transform, mainCamera);
                position.y = OFFSET_Y;
                transform.position = position;
                break;
            case CardState.FIELD:
                // 攻撃の線を出す
                SetLineRenderer();
                break;
            default: break;
        }
    }

    private void OnMouseUp()
    {
        if (!isLocal || !BattleManager.instance.IsOwnTurn()) return;
        switch (currentState)
        {
            case CardState.HAND:
                // オブジェクトをUIにセット
                UIManager.instance.DropCard(this);
                break;
            case CardState.FIELD:
                lineRenderer.enabled = false;
                // 攻撃処理
                Attack();
                break;
            default: break;
        }
    }

    /// <summary>
    /// 攻撃線の設定
    /// </summary>
    private void SetLineRenderer()
    {
        // 制御点の総数
        int totalPointNum = LINE_CONTROL_POINT_NUM + 2;
        // 始点終点
        Vector3 startPoint = transform.position;
        Vector3 endPoint = GetMouseWorldPosition(transform, mainCamera);
        // 中間点を曲線が見えるようにずらす
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 midPoint = (startPoint + endPoint) / 2 + Vector3.up * LINE_HEIGHT + cameraUp * LINE_OFFSET;

        // 始点登録
        lineRenderer.SetPosition(0, startPoint);
        for (int i = 1; i <= LINE_CONTROL_POINT_NUM; i++)
        {
            float t = (float)i / (float)(totalPointNum - 1);
            Vector3 point = GetBezierCurve2(startPoint, endPoint, midPoint, t);
            lineRenderer.SetPosition(i, point);
        }
        // 終点登録
        lineRenderer.SetPosition(totalPointNum - 1, endPoint);

        // 太さ設定
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        // マウスの座標からオブジェクトを取得
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject hitObject = null;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            hitObject = hit.collider.gameObject;
            if (hitObject == null) return;
        }
        BaseFieldObject target = hitObject.GetComponent<BaseFieldObject>();
        if (target == null) return;
        // 自分自身は攻撃できない
        if (target.isLocal) return;
        // 攻撃可能オブジェクトか判定(フィールドに出ている敵フォロワーか敵リーダー)
        CardObject targetCard = target as CardObject;
        if (targetCard != null)
            AttackFollower(targetCard);

        LeaderObject targetLeader = target as LeaderObject;
        if (targetLeader != null)
            AttackLeader(targetLeader);
    }

    /// <summary>
    /// フォロワーへの攻撃
    /// </summary>
    /// <param name="targetCard"></param>
    private void AttackFollower(CardObject targetCard)
    {
        // フォロワー以外は攻撃できない
        if (targetCard.cardData.type != GameEnum.CardType.FOLLOWER) return;
        // 情報を送信
        int sourceIndex = UIManager.instance.GetOwnFieldIndex(this);
        int targetIndex = UIManager.instance.GetOpponentFieldIndex(targetCard);
        BattleManager.instance.SendInputData(GameEnum.InputType.ATTACK_FOLLOWER, new int[2] { sourceIndex, targetIndex });
        // 攻撃処理を依頼
        BattleManager.instance.CardCombat(cardData, targetCard.cardData);
    }

    /// <summary>
    /// リーダーへの攻撃
    /// </summary>
    /// <param name="leaderCard"></param>
    private void AttackLeader(LeaderObject leaderCard)
    {
        // 情報を送信
        int sourceIndex = UIManager.instance.GetOwnFieldIndex(this);
        BattleManager.instance.SendInputData(GameEnum.InputType.ATTACK_LEADER, new int[1] { sourceIndex });
        // 攻撃処理を依頼
        BattleManager.instance.LeaderCombat(cardData, leaderCard.leader);
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
                cardObject[(int)CardState.HAND].SetActive(true);
                break;
            case CardState.FIELD:
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(true);
                break;
            case CardState.UNUSE:
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(false);
                break;
            default: break;
        }
    }

    /// <summary>
    /// カードの見た目の適用
    /// </summary>
    public void SetCardLook()
    {
        // オブジェクト設定
        switch (cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_FOLLOWER];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.FIELD_FOLLOWER];
                break;
            case GameEnum.CardType.SPELL:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_SPELL];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.HAND_SPELL];
                break;
            case GameEnum.CardType.AMULET:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_AMULET];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.FIELD_AMULET];
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
    }

    public void FlipCard()
    {
        transform.DORotate(new Vector3(0, 0, 180), 0.5f, RotateMode.LocalAxisAdd);
    }

    /// <summary>
    /// 自分のドロー
    /// </summary>
    /// <param name="deckRoot"></param>
    /// <param name="drawRoot"></param>
    /// <param name="handRoot"></param>
    /// <returns></returns>
    public Sequence DrawOwnCard(Transform deckRoot, Transform drawRoot, Transform handRoot)
    {
        // ドロールートまでの挙動
        Sequence drawSeq = DOTween.Sequence();
        drawSeq.AppendCallback(() => transform.position = deckRoot.position)
            .JoinCallback(() => transform.rotation = deckRoot.rotation)
            .JoinCallback(() => gameObject.SetActive(true))
            .Join(transform.DOMove(drawRoot.position, 0.5f))
            .Join(transform.DORotate(drawRoot.localEulerAngles, 0.5f));
        // 手札ルートまでの挙動
        drawSeq.Append(transform.DORotate(handRoot.localEulerAngles, 0.5f))
            .Join(transform.DOMove(handRoot.position, 0.5f))
            .JoinCallback(() => transform.SetParent(handRoot));
        return drawSeq;
    }

    /// <summary>
    /// 相手のドロー
    /// </summary>
    /// <param name="deckRoot"></param>
    /// <param name="handRoot"></param>
    /// <returns></returns>
    public Sequence DrawOpponentCard(Transform deckRoot, Transform handRoot)
    {
        Sequence drawSeq = DOTween.Sequence();
        // 手札ルートまでの挙動
        drawSeq.AppendCallback(() => transform.position = deckRoot.position)
            .JoinCallback(() => transform.rotation = deckRoot.rotation)
            .JoinCallback(() => gameObject.SetActive(true))
            .Join(transform.DORotate(handRoot.localEulerAngles, 0.5f))
            .Join(transform.DOMove(handRoot.position, 0.5f))
            .JoinCallback(() => transform.SetParent(handRoot));
        return drawSeq;
    }

    public void PlayCard()
    {
        // 選択が必要な能力なら選択ウィンドウを出す
        // 何かしらに渡す

        Hand currentHand = BattleManager.instance.GetCurrentPlayer().hand;
        currentHand.PlayCardToField(cardData);
    }

    public void EvolveFollower()
    {
        // 進化前挙動


        // 進化後挙動

    }

    public void SuperEvolveFollower()
    {
        // 進化前挙動


        // 進化後挙動

    }

    public void DestroyCard()
    {
        // 破壊エフェクト

        // オブジェクト非表示
        SetCardState(CardState.UNUSE);

        // フィールドから除外
        UIManager.instance.RemoveFieldCard(this);
    }
}
