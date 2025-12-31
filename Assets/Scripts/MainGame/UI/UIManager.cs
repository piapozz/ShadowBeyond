using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardObject;
using static CommonModule;

// UIを管理するマネージャー

// 管理するUI
// ・手札UI
// ・ターンUI
// ・デッキUI
// ・フィールドUI
// ・リーダーUI
// ・オプションUI
// ・バトル履歴UI
// ・PPUI
// ・バトル情報UI
public class UIManager : SystemObject
{
    [SerializeField] private Transform UICanvas;
    [SerializeField] private Transform worldCanvas;

    [SerializeField] private CardObject cardObject;
    [SerializeField] private HandUI handUI;
    [SerializeField] private FieldUI fieldUI;
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private TurnEndUI turnUI;
    [SerializeField] private PPUI ownPPUI;
    [SerializeField] private PPUI opponentPPUI;
    //[SerializeField] private OptionUI optionUI;
    //[SerializeField] private HistoryUI historyUI;
    //[SerializeField] private InfoUI infoUI;
    [SerializeField] private LeaderUI leaderUI;
    [SerializeField] private GameObject ownDeckObject;
    [SerializeField] private GameObject opponentDeckObject;
    [SerializeField] private Transform playCardRoot;
    [SerializeField] private CardDetailUI cardDetailUI;
    [SerializeField] private ReadyUI readyUI;

    public enum UIState
    {
        INVALID = -1,
        DEFAULT,
        ATTACK,
        MAX
    }

    public static UIManager instance { get; private set; }

    public UIState state { get; private set; } = UIState.DEFAULT;
    private Queue<List<Sequence>> uiSequence = null;
    private List<Sequence> currentSequenceList = null;
    private List<CardObject> poolCardObject = null;
    private Camera mainCamera = null;

    private const int POOL_CARD_NUM = 30;
    // 攻撃線の制御点数
    private const int LINE_CONTROL_POINT_NUM = 20;
    // 攻撃線の高さ
    private const float LINE_HEIGHT = 5.0f;
    // 攻撃線のずらし幅
    private const float LINE_OFFSET = 2.0f;


    public override async UniTask Initialize()
    {
        // DOTween初期化
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;

        // UIキャンバス取得
        var worldCanvasObj = worldCanvas.gameObject.GetComponent<Canvas>();
        worldCanvasObj.worldCamera = Camera.main;

        instance = this;
        mainCamera = Camera.main;
        uiSequence = new Queue<List<Sequence>>();
        currentSequenceList = new List<Sequence>();
        // UIを生成
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);
        ownDeckObject = Instantiate(ownDeckObject);
        opponentDeckObject = Instantiate(opponentDeckObject);
        leaderUI = Instantiate(leaderUI);

        // カードオブジェクトをプール
        poolCardObject = new List<CardObject>(POOL_CARD_NUM);
        for (int i = 0; i < POOL_CARD_NUM; i++)
        {
            CardObject card = Instantiate(cardObject);
            card.transform.SetParent(transform);
            card.gameObject.SetActive(false);
            poolCardObject.Add(card);
        }
    }

    private void Update()
    {
        // UIシーケンス処理
        UISequence();

        CardClick();
    }

    private void UISequence()
    {
        // UIシーケンス処理
        if (IsCompleteAllSequence()) return;
        // シーケンスがたまっていて、現在のシーケンスが終了していたら次のシーケンスを再生
        if (IsCompleteCurrentSequence())
        {
            if (uiSequence.Count == 0) return;
            currentSequenceList = uiSequence.Dequeue();
            for (int i = 0, max = currentSequenceList.Count; i < max; i++)
            {
                currentSequenceList[i].Play();
            }
        }
    }

    private void CardClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        BaseFieldObject target = GetFieldObject(Input.mousePosition);
        if (target != null && target as CardObject)
        {
            CardObject card = target as CardObject;
            // 相手の手札はクリックできない
            if (!card.isLocal && card.currentState == CardState.HAND) return;
            cardDetailUI.EnableUI(true, card.cardData.name, card.cardData.text);
            return;
        }
        cardDetailUI.EnableUI(false);
    }

    public BaseFieldObject GetFieldObject(Vector2 screenPos)
    {
        // カードの詳細画面表示
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        GameObject hitObject = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            hitObject = hit.collider.gameObject;
        }
        if (hitObject == null) return null;
        BaseFieldObject target = hitObject.GetComponent<BaseFieldObject>();
        return target;
    }

    private bool IsCompleteCurrentSequence()
    {
        for (int i = 0, max = currentSequenceList.Count; i < max; i++)
        {
            if (!currentSequenceList[i].IsActive()) continue;

            if (currentSequenceList[i].IsActive() && !currentSequenceList[i].IsComplete()) return false;
        }
        currentSequenceList.Clear();
        return true;
    }

    public void AddSequence(Sequence addSequence)
    {
        if (addSequence == null) return;
        List<Sequence> sequenceList = new List<Sequence>(1);
        sequenceList.Add(addSequence);
        uiSequence.Enqueue(sequenceList);
    }

    public void AddSequence(List<Sequence> addSequences)
    {
        if (addSequences == null) return;
        uiSequence.Enqueue(addSequences);
    }

    public bool IsCompleteAllSequence()
    {
        return uiSequence.Count == 0 && currentSequenceList.Count == 0;
    }

    public void SetUIState(UIState setState)
    {
        state = setState;
    }

    // ターン開始
    public void StartTurn(bool isOwnTurn)
    {
        // ターン終了ボタンの設定
        turnUI.SetTurnEndButton(isOwnTurn);
    }

    /// <summary>
    /// 未使用のカードオブジェクトを取得
    /// </summary>
    /// <returns></returns>
    private CardObject GetUnuseCardObject()
    {
        for (int i = 0, max = poolCardObject.Count; i < max; i++)
        {
            cardObject = poolCardObject[i];
            if (cardObject.gameObject.activeSelf) continue;
            return cardObject;
        }
        return Instantiate(cardObject);
    }

    /// <summary>
    /// オブジェクト参照の自身の手札のインデックス取得
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int GetOwnHandIndex(CardObject card)
    {
        return handUI.GetOwnHandIndex(card);
    }

    public int GetOwnFieldIndex(CardObject card)
    {
        return fieldUI.GetOwnFieldIndex(card);
    }

    public int GetOpponentFieldIndex(CardObject card)
    {
        return fieldUI.GetOpponentFieldIndex(card);
    }

    public CardObject GetOpponentCard(int index)
    {
        return fieldUI.GetOpponentCard(index);
    }

    /// <summary>
    /// カードがドロップされたときの処理
    /// </summary>
    /// <param name="setCard"></param>
    public void DropCard(CardObject setCard)
    {
        Vector3 mousePos = Input.mousePosition;

        // フィールド領域か判定
        bool isField = RectTransformUtility.RectangleContainsScreenPoint(fieldArea, mousePos);
        // カードをプレイ
        if (isField)
        {
            PlayOwnCard(setCard);
        }
        // 手札に戻す
        else
        {
            AddSequence(handUI.ArrangeHandCard(true));
        }
    }

    /// <summary>
    /// オブジェクト指定のカードをプレイ
    /// </summary>
    /// <param name="playCard"></param>
    /// <param name="isMine"></param>
    public void PlayOwnCard(CardObject playCard)
    {
        // 手札から除外しフィールドに追加
        int handIndex =　handUI.GetOwnHandIndex(playCard);
        handUI.RemoveHandCard(true, playCard);
        // 手札整列と手札から出すアニメーション登録
        SetCardPlaySequence(true, playCard);

        switch (playCard.cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
            case GameEnum.CardType.AMULET:
                // UIの挙動
                fieldUI.PlayFieldCard(true, playCard);
                break;
            case GameEnum.CardType.SPELL:
                // UIの挙動
                fieldUI.PlaySpellCard(true, playCard);
                break;
            default: break;
        }
        
        // 送信
        int[] param = new int[1] { handIndex };
        BattleManager.instance.SendInputData(GameEnum.InputType.PLAY_CARD, param);
    }

    public void PlayOpponentCard(int handIndex)
    {
        // 手札から除外しフィールドに追加
        CardObject playCard = handUI.GetOpponentCardObject(handIndex);
        handUI.RemoveHandCard(false, playCard);
        // 手札整列と手札から出すアニメーション登録
        SetCardPlaySequence(false, playCard);

        switch (playCard.cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
            case GameEnum.CardType.AMULET:
                fieldUI.PlayFieldCard(false, playCard);
                break;
            case GameEnum.CardType.SPELL:
                fieldUI.PlaySpellCard(false, playCard);
                break;
            default: break;
        }
    }

    private void SetCardPlaySequence(bool isOwn, CardObject playCard)
    {
        // 手札整列と手札から出すアニメーション登録
        List<Sequence> playSeq = new List<Sequence>();
        List<Sequence> arrangeSeq = handUI.ArrangeHandCard(isOwn);
        if (arrangeSeq != null) playSeq.AddRange(arrangeSeq);
        playSeq.Add(playCard.GetPlaySequence(playCardRoot));
        AddSequence(playSeq);
    }

    /// <summary>
    /// デッキからカードをドローする
    /// </summary>
    /// <param name="isMine"></param>
    /// <param name="drawCard"></param>
    public void DrawCards(int playerID, List<CardData> drawCard)
    {
        int drawCardNum = drawCard.Count;
        List<CardObject> drawCardObjects = new List<CardObject>(drawCardNum);
        for (int i = 0; i < drawCardNum; i++)
        {
            CardObject cardObject = GetUnuseCardObject();
            // カードデータセット
            cardObject.SetCardData(drawCard[i]);
            cardObject.SetCardState(CardObject.CardState.HAND);
            drawCardObjects.Add(cardObject);
        }
        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        Transform deckTransform = isMine ? ownDeckObject.transform : opponentDeckObject.transform;
        handUI.DrawCard(isMine, drawCardObjects, deckTransform);
    }

    public void ShuffleDeck(int playerID)
    {

    }

    public void PlayDeckToField(int playerID, CardData playCard)
    {

    }

    public void RemoveDeckCards(int playerID)
    {

    }

    public void SetEndTurnButton(Action setAction)
    {
        turnUI.SetButtonAction(setAction);
    }

    public void UpdatePPUI(int playerID, int ppMax, int ppCurrent)
    {
        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        if (isMine) ownPPUI.SetPPText(ppMax, ppCurrent);
        else opponentPPUI.SetPPText(ppMax, ppCurrent);
    }

    public void RemoveFieldCard(CardObject card)
    {
        if (card.isLocal)
        {
            fieldUI.RemoveOwnFieldCard(card);
        }
        else
        {
            fieldUI.RemoveOpponentFieldCard(card);
        }
    }

    public void SetLeader(Leader setLeader, int index)
    {
        leaderUI.Initialize(setLeader, index);
    }

    /// <summary>
    /// フォロワーへの攻撃挙動の設定
    /// </summary>
    /// <param name="sourceCard"></param>
    /// <param name="targetCard"></param>
    public void SetAttackFollowerSequence(CardObject sourceCard, CardObject targetCard)
    {
        Sequence attackSequence = DOTween.Sequence();
        // 持ち上げる
        attackSequence.Append(sourceCard.GetPickupSequence(true))
            .Join(targetCard.GetPickupSequence(true));
        // 攻撃
        attackSequence.Append(sourceCard.GetAttackSequence());
        // 反撃
        attackSequence.Append(targetCard.GetCounterAttackSequence());
        // 防御
        attackSequence.Append(sourceCard.GetDefenceSequence());
        attackSequence.AppendCallback(() => sourceCard.CheckDestroyCard())
            .JoinCallback(() => targetCard.CheckDestroyCard());
        // おろす
        attackSequence.Append(sourceCard.GetPickupSequence(false))
            .Join(targetCard.GetPickupSequence(false));

        AddSequence(attackSequence);
    }

    public void SetAttackLeaderSequence(CardObject sourceCard)
    {
        Sequence attackSequence = DOTween.Sequence();
        // 持ち上げる
        attackSequence.Append(sourceCard.GetPickupSequence(true));
        // 攻撃
        attackSequence.Append(sourceCard.GetAttackSequence());
        // おろす
        attackSequence.Append(sourceCard.GetPickupSequence(false));

        AddSequence(attackSequence);
    }

    public void SetDefenceFollower(int targetIndex, int sourceIndex)
    {
        CardObject targetCard = fieldUI.GetOwnCard(targetIndex);
        CardObject sourceCard = fieldUI.GetOpponentCard(sourceIndex);

        SetAttackFollowerSequence(sourceCard, targetCard);
    }

    /// <summary>
    /// 選択線の設定
    /// </summary>
    /// <param name="lineRenderer"></param>
    /// <param name="setTransform"></param>
    /// <param name="controlPointNum"></param>
    /// <param name="lineHeight"></param>
    /// <param name="lineOffset"></param>
    public void SetLineRenderer(LineRenderer lineRenderer,　Transform setTransform, int controlPointNum = LINE_CONTROL_POINT_NUM, float lineHeight = LINE_HEIGHT, float lineOffset = LINE_OFFSET)
    {
        // 制御点の総数
        int totalPointNum = controlPointNum + 2;
        // 始点終点
        Vector3 startPoint = setTransform.position;
        Vector3 endPoint = CommonModule.GetMouseWorldPosition(setTransform, mainCamera);
        // 中間点を曲線が見えるようにずらす
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 midPoint = (startPoint + endPoint) / 2 + Vector3.up * lineHeight + cameraUp * lineOffset;

        // 始点登録
        lineRenderer.SetPosition(0, startPoint);
        for (int i = 1; i <= controlPointNum; i++)
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

    public Vector3 GetMouseWorldPosition(Transform transform)
    {
        return CommonModule.GetMouseWorldPosition(transform, mainCamera);
    }

    // バトル開始時演出
    public async UniTask PlayStartBattleSequence(int currntPlayer)
    {
        // TODO: バトル開始時演出
        HideUI();
        readyUI.gameObject.SetActive(true);

        // キャラだし
        readyUI.Initialize("Player1", "ニュートラル", "Player2", "ニュートラル");

        // 順番決め演出
        await readyUI.MoveOrderCard(currntPlayer);

        // UI破棄
        Destroy(readyUI.gameObject);

        // マリガン


        // UI各種表示
        ShowUI();
        readyUI.gameObject.SetActive(false);


        return;
    }

    public void OnGUI()
    {
        // デバッグ用UI
        // シーケンス数
        GUI.Label(new Rect(10, 10, 1000, 100), "UI Sequence Count: " + uiSequence.Count);
    }

    public void HideUI()
    {
        handUI.gameObject.SetActive(false);
        fieldUI.gameObject.SetActive(false);
        turnUI.gameObject.SetActive(false);
        ownPPUI.gameObject.SetActive(false);
        opponentPPUI.gameObject.SetActive(false);
        //optionUI.gameObject.SetActive(false);
        //historyUI.gameObject.SetActive(false);
        //infoUI.gameObject.SetActive(false);
        leaderUI.gameObject.SetActive(false);
        cardDetailUI.gameObject.SetActive(false);
        readyUI.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        handUI.gameObject.SetActive(true);
        fieldUI.gameObject.SetActive(true);
        turnUI.gameObject.SetActive(true);
        ownPPUI.gameObject.SetActive(true);
        opponentPPUI.gameObject.SetActive(true);
        //optionUI.gameObject.SetActive(true);
        //historyUI.gameObject.SetActive(true);
        //infoUI.gameObject.SetActive(true);
        leaderUI.gameObject.SetActive(true);
        cardDetailUI.gameObject.SetActive(true);
        readyUI.gameObject.SetActive(true);
    }

}

