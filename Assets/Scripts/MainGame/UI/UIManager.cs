using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    [SerializeField] private RedrawUI redrawUI;
    [SerializeField] private MessageUI messageUI;
    [SerializeField] private Canvas backCanvas;

    public enum UIState
    {
        INVALID = -1,
        DEFAULT,
        ATTACK,
        SELECT,
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
        backCanvas.worldCamera = mainCamera;
        backCanvas.planeDistance = 15;

        uiSequence = new Queue<List<Sequence>>();
        currentSequenceList = new List<Sequence>();
        // UIを生成
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);
        ownDeckObject = Instantiate(ownDeckObject);
        opponentDeckObject = Instantiate(opponentDeckObject);
        leaderUI = Instantiate(leaderUI);
        messageUI = Instantiate(messageUI);

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

        CheckClick();
    }

    public async UniTask Message(string message, float sec)
    {
        await messageUI.MessageText(message, sec);
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

    public void SetCardDetailUI(CardObject cardObject)
    {
        CardData cardData = cardObject.GetCardData();
        // アクトを持っていて、場にあるならボタンを登録
        Action actAction = null;
        if (cardData.HaveKeyword(GameEnum.KeywordAbility.Engage) && 
            cardObject.currentState == CardState.FIELD)
        {
            actAction = () =>
            {
                cardDetailUI.EnableUI(false);
                CardActionExecutor.TryAct(cardData, cardObject.isLocal);
            };
        }
        // 融合を持っていて、手札にあるなら、ボタンを登録
        Action fusionAction = null;
        if (cardData.HaveKeyword(GameEnum.KeywordAbility.Fuse) &&
            cardObject.currentState == CardState.HAND)
        {
            fusionAction = () =>
            {
                cardData.ability.Fuse(cardObject.isLocal);
                cardDetailUI.EnableUI(false);
            };
        }
        bool isOwnTurn = BattleManager.instance.IsOwnTurn();
        cardDetailUI.EnableUI(true, cardData, actAction, fusionAction, isOwnTurn);
    }

    /// <summary>
    /// カード以外のクリックを検知
    /// </summary>
    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // UIがクリックされているならスキップ
        if (IsClickDetailUI()) return;

        BaseFieldObject target = GetFieldObject(Input.mousePosition);
        switch (state)
        {
            case UIState.DEFAULT:
                ClickDefaultState(target);
                break;
            case UIState.SELECT:
                ClickSelectState(target);
                break;
            default: break;
        }
    }

    private void ClickDefaultState(BaseFieldObject clickObject)
    {
        CardObject cardObject = clickObject as CardObject;
        // カードオブジェクトでないなら詳細画面を閉じる
        if (clickObject == null || cardObject == null)
        {
            cardDetailUI.EnableUI(false);
            return;
        }
        // 相手の手札は詳細画面を出さない
        if (clickObject.isLocal || cardObject.currentState == CardState.FIELD) return;
        cardDetailUI.EnableUI(false);
    }

    private void ClickSelectState(BaseFieldObject clickObject)
    {
        // オブジェクト以外がクリックされたらキャンセル処理へ
        if (clickObject == null)
        {
            Debug.Log("Select cancel");
            CompleteSelection(false);
        }
        // オブジェクトがクリックされていたら選択処理へ
        else
        {
            if (!clickObject.isSelectable) return;
            Debug.Log("Select success");
            clickObject.OnPointerClick();
        }
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

    private bool IsClickDetailUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // CardDetailUI 自身、または子オブジェクトなら true
            if (result.gameObject.GetComponentInParent<CardDetailUI>() == cardDetailUI)
            {
                return true;
            }
        }

        return false;
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

    public async UniTask IsCompleteAllSequenceTask()
    {
        while (!IsCompleteAllSequence())
        {
            await UniTask.Yield();
        }
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

    // ターン終了
    public void EndTurn()
    {
        cardDetailUI.EnableUI(false);
    }

    /// <summary>
    /// 未使用のカードオブジェクトを取得
    /// </summary>
    /// <returns></returns>
    public CardObject GetUnuseCardObject()
    {
        for (int i = 0, max = poolCardObject.Count; i < max; i++)
        {
            cardObject = poolCardObject[i];
            if (cardObject.currentState != CardState.UNUSE) continue;
            return cardObject;
        }
        return Instantiate(cardObject);
    }

    /// <summary>
    /// ID指定で新しいカードオブジェクトを取得
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public CardObject GetNewCardObject(int cardId)
    {
        CardObject newCard = GetUnuseCardObject();
        newCard.SetCardData(CardMasterUtility.GetCardData(cardId));
        return newCard;
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
            // プレイ
            PlayOwnCard(setCard);
        }
        // 手札に戻す
        else
        {
            AddSequence(handUI.ArrangeHandCard(true));
        }
    }

    /// <summary>
    /// カードがマリガン中にドロップされたときの処理
    /// </summary>
    /// <param name="setCard"></param>
    public async UniTask RedrawDropCard(CardObject setCard)
    {
       await redrawUI.IsInRedrawArea(setCard);
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

        switch (playCard.GetCardData().type)
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

        switch (playCard.GetCardData().type)
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
            drawCardObjects.Add(cardObject);
        }
        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        Transform deckTransform = isMine ? ownDeckObject.transform : opponentDeckObject.transform;
        handUI.DrawCard(isMine, drawCardObjects, deckTransform);
    }

    /// <summary>
    /// デッキからカードをドローする
    /// </summary>
    /// <param name="isMine"></param>
    /// <param name="drawCard"></param>
    public void InsertDrawCards(int playerID, CardData drawCard, int index)
    {
        CardObject drawCardObject;
        CardObject cardObject = GetUnuseCardObject();
        // カードデータセット
        cardObject.SetCardData(drawCard);
        cardObject.SetCardState(CardState.HAND);
        drawCardObject = cardObject;

        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        Transform deckTransform = isMine ? ownDeckObject.transform : opponentDeckObject.transform;
        handUI.InsertDrawCard(isMine, drawCardObject, deckTransform, index);
    }

    public List<Sequence> GetInsertDrawCardSequence(int playerID, CardData drawCard, int index)
    {
        CardObject drawCardObject;
        CardObject cardObject = GetUnuseCardObject();
        // カードデータセット
        cardObject.SetCardData(drawCard);
        cardObject.SetCardState(CardState.HAND);
        drawCardObject = cardObject;

        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        Transform deckTransform = isMine ? ownDeckObject.transform : opponentDeckObject.transform;
        return handUI.GetInsertDrawCardSequence(isMine, drawCardObject, deckTransform, index);
    }

    /// <summary>
    /// デッキにカードを戻す
    /// </summary>
    /// <param name="isMine"></param>
    /// <param name="drawCard"></param>
    public void ReturnCards(int playerID, List<CardData> returnCard)
    {
        int returnCardNum = returnCard.Count;
        List<CardObject> drawCardObjects = new List<CardObject>(returnCardNum);
        for (int i = 0; i < returnCardNum; i++)
        {
            CardObject cardObject = returnCard[i].GetCardObject();
            // カードデータセット
            cardObject.SetCardData(returnCard[i]);
            drawCardObjects.Add(cardObject);
        }
        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        Transform deckTransform = isMine ? ownDeckObject.transform : opponentDeckObject.transform;
        handUI.ReturnCardDeck(isMine, drawCardObjects, deckTransform);
    }

    // 手札にカードを加える
    public void AddHandCard(int playerID, List<CardData> addCard)
    {
        int addCardNum = addCard.Count;
        List<CardObject> drawCardObjects = new List<CardObject>(addCardNum);
        for (int i = 0; i < addCardNum; i++)
        {
            CardObject cardObject = addCard[i].GetCardObject();
            // カードデータセット
            cardObject.SetCardData(addCard[i]);
            cardObject.SetCardState(CardObject.CardState.HAND);
            drawCardObjects.Add(cardObject);
        }
        bool isMine = playerID == (int)GameEnum.PlayerType.OWN;
        handUI.AddHandCard(isMine, drawCardObjects);
    }

    public void EnterFieldSequence(List<CardObject> enterCards, bool isOwn)
    {
        fieldUI.EnterFieldCard(isOwn, enterCards);
    }

    public void SetBounceSequence(List<CardObject> bounceCards, bool isOwn)
    {
        fieldUI.BounceCard(isOwn, bounceCards);
    }

    public void SetReturnDeckSequence(List<CardObject> bounceCards, bool isOwn)
    {
        handUI.ReturnCardDeck(isOwn, bounceCards, isOwn ? ownDeckObject.transform : opponentDeckObject.transform);
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
        fieldUI.RemoveFieldCard(card, card.isLocal);
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
        // バトル開始時演出
        HideUI();
        redrawUI.gameObject.SetActive(false);
        readyUI.gameObject.SetActive(true);
        // キャラだし
        readyUI.Initialize("あなた", " ", "あいて", "　");
        // 順番決め演出
        await readyUI.MoveOrderCard(currntPlayer);
        // UI非表示
        readyUI.gameObject.SetActive(false);

        // マリガン
        // カードを四枚取得マリガンに渡す
        redrawUI.gameObject.SetActive(true);
        List<CardData> drawCards;
        var player = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OWN);

        drawCards = player.deck.PeekDeck(4);
        for (int i = 0; i < drawCards.Count; i++)
        {
            CardObject cardObject = GetUnuseCardObject();
            // カードデータセット
            cardObject.SetCardData(drawCards[i]);
            cardObject.SetCardState(CardState.REDRAW);
        }
        redrawUI.StartRedraw(drawCards, ownDeckObject.transform);

        while (redrawUI.IsRedraw())
        {
            await UniTask.Yield();
        }
        redrawUI.gameObject.SetActive(false);

        // UI各種表示
        ShowUI();

        return;
    }

    public class TargetSelectResult
    {
        public bool result;
        public List<BaseComponent> selected;
    }
    private UniTaskCompletionSource<TargetSelectResult> _selectTcs;

    private int _selectCount;
    private List<BaseFieldObject> _candidates;
    private List<BaseComponent> _selected = new();

    public UniTask<TargetSelectResult> SelectTargetAsync(
    List<BaseComponent> candidates, int selectCount)
    {
        // 二重選択防止
        if (_selectTcs != null)
        {
            Debug.Log("二重選択");
        }


        // オブジェクトのリストに変換
        int targetObjectCount = candidates.Count;
        List<BaseFieldObject> targetObjects = new List<BaseFieldObject>(targetObjectCount);
        for (int i = 0, max = targetObjectCount; i < max; i++)
        {
            BaseFieldObject obj = candidates[i].GetObject();
            targetObjects.Add(obj);
        }

        _selectCount = selectCount;
        _candidates = targetObjects;
        _selected.Clear();

        _selectTcs = new UniTaskCompletionSource<TargetSelectResult>();

        // 入力状態を変更
        SetUIState(UIState.SELECT);
        // 背景を暗くする
        backCanvas.enabled = true;

        // 対象を選択可能にする
        foreach (var card in _candidates)
        {
            card.EnableSelectable(true);
            card.OnClick += OnCardClicked;
        }

        return _selectTcs.Task;
    }

    private void OnCardClicked(BaseFieldObject fieldObject)
    {
        if (_selectTcs == null) return;

        // 候補にない場合は無視
        if (!_candidates.Contains(fieldObject)) return;

        // 既に選択されている場合はトグル
        if (_selected.Contains(fieldObject.component))
        {
            _selected.Remove(fieldObject.component);
            fieldObject.SetSelected(false);
            return;
        }
        // 新規選択
        _selected.Add(fieldObject.component);
        fieldObject.SetSelected(true);

        // 必要枚数選ばれたら完了
        if (_selected.Count >= _selectCount)
        {
            CompleteSelection(true);
        }
    }

    private void CompleteSelection(bool isComplete)
    {
        List<BaseComponent> selected = isComplete ? _selected : null;
        _selectTcs.TrySetResult(new TargetSelectResult
        {
            result = isComplete,
            selected = selected
        });

        _selectTcs = null;
        Cleanup();
    }

    private void Cleanup()
    {
        foreach (var card in _candidates)
        {
            card.EnableSelectable(false);
            card.OnClick -= OnCardClicked;
        }

        _selectCount = 0;
        _candidates = null;
        _selected.Clear();

        SetUIState(UIState.DEFAULT);
        backCanvas.enabled = false;
    }

    public void OnGUI()
    {
        // デバッグ用UI
        // シーケンス数
        GUI.Label(new Rect(10, 10, 1000, 100), "UI Sequence Count: " + uiSequence.Count);
    }

    public void HideUI()
    {
        //handUI.gameObject.SetActive(false);
        fieldUI.gameObject.SetActive(false);
        turnUI.gameObject.SetActive(false);
        ownPPUI.gameObject.SetActive(false);
        opponentPPUI.gameObject.SetActive(false);
        //optionUI.gameObject.SetActive(false);
        //historyUI.gameObject.SetActive(false);
        //infoUI.gameObject.SetActive(false);
        leaderUI.gameObject.SetActive(false);
        cardDetailUI.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        //handUI.gameObject.SetActive(true);
        fieldUI.gameObject.SetActive(true);
        turnUI.gameObject.SetActive(true);
        ownPPUI.gameObject.SetActive(true);
        opponentPPUI.gameObject.SetActive(true);
        //optionUI.gameObject.SetActive(true);
        //historyUI.gameObject.SetActive(true);
        //infoUI.gameObject.SetActive(true);
        leaderUI.gameObject.SetActive(true);
        cardDetailUI.gameObject.SetActive(true);
    }

}

