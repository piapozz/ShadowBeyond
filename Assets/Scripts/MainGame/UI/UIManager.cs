using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static NetworkManager;

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

    [SerializeField] private CardObject cardObject;
    [SerializeField] private HandUI handUI;
    [SerializeField] private FieldUI fieldUI;
    [SerializeField] private LeaderUI leaderUI;
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private Button turnUI;
    [SerializeField] private PPUI ownPPUI;
    [SerializeField] private PPUI opponentPPUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private HistoryUI historyUI;
    [SerializeField] private InfoUI infoUI;
    [SerializeField] private GameObject ownDeckObject;
    [SerializeField] private GameObject opponentDeckObject;
    [SerializeField] private LeaderObject ownLeaderObject;
    [SerializeField] private LeaderObject opponentLeaderObject;

    public static UIManager instance { get; private set; }

    public Queue<List<Sequence>> uiSequence { get; private set; } = null;
    private List<Sequence> currentSequenceList = null;

    private UniTaskCompletionSource _uniTaskCompletionSource = null;

    private List<CardObject> poolCardObject = null;

    private const int POOL_CARD_NUM = 30;

    public override async UniTask Initialize()
    {
        // DOTween初期化
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;

        instance = this;
        uiSequence = new Queue<List<Sequence>>();
        currentSequenceList = new List<Sequence>();
        // UIを生成
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);
        ownDeckObject = Instantiate(ownDeckObject);
        opponentDeckObject = Instantiate(opponentDeckObject);

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
        if (IsCompleteAllSequence()) return;
        // シーケンスがたまっていて、現在のシーケンスが終了していたら次のシーケンスを再生
        if (IsCompleteCurrentSequence())
        {
            currentSequenceList = uiSequence.Dequeue();
            for (int i = 0, max = currentSequenceList.Count; i < max; i++)
            {
                currentSequenceList[i].Play();
            }
        }

        // 情報UI更新
        // OptionUI更新
        // HistoryUI更新
    }

    private bool IsCompleteCurrentSequence()
    {
        for (int i = 0, max = currentSequenceList.Count; i < max; i++)
        {
            if (!currentSequenceList[i].IsActive()) continue;

            if (currentSequenceList[i].IsActive() && !currentSequenceList[i].IsComplete()) return false;
        }
        return true;
    }

    public void AddSequence(List<Sequence> addSequences)
    {
        if (addSequences == null) return;
        uiSequence.Enqueue(addSequences);
    }

    public bool IsCompleteAllSequence()
    {
        return uiSequence.Count == 0;
    }

    public void StartBattle()
    {
        // 各UI表示

    }

    // ターン開始
    public void StartTurn(int playerIndex)
    {
        // ターンUI更新

        // ドロー処理

        // 手札UI更新

        // PPUI更新

        // リーダーUI更新

        // 手札、フィールド、ボタンの操作可能化
    }

    // ターン終了
    public void EndTurn()
    {
        // ターンUI更新

        // 手札、フィールド、ボタンの操作不可化
    }

    // Unitaskでなにかがおこるまで待つ関数
    public async UniTask<SendBattleData> InputUI()
    {
        _uniTaskCompletionSource = new UniTaskCompletionSource();



        await _uniTaskCompletionSource.Task;
        return new SendBattleData();
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
            handUI.ArrangeHandCard(true);
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
        switch (playCard.cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
            case GameEnum.CardType.AMULET:
                fieldUI.AddOwnFieldCard(playCard);
                break;
            case GameEnum.CardType.SPELL:
                playCard.SetCardState(CardObject.CardState.UNUSE);
                break;
            default: break;
        }
        playCard.PlayCard();
        int[] param = new int[1] { handIndex };
        BattleManager.instance.SendInputData(GameEnum.InputType.PLAY_CARD, param);
    }

    public void PlayOpponentCard(int handIndex)
    {
        // 手札から除外しフィールドに追加
        CardObject playCard = handUI.GetOpponentCardObject(handIndex);
        handUI.RemoveHandCard(false, playCard);
        switch (playCard.cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
            case GameEnum.CardType.AMULET:
                fieldUI.AddOpponentFieldCard(playCard);
                break;
            case GameEnum.CardType.SPELL:
                playCard.SetCardState(CardObject.CardState.UNUSE);
                break;
            default: break;
        }
        playCard.PlayCard();
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
            cardObject.SetCardData(drawCard[0]);
            cardObject.SetCardState(CardObject.CardState.HAND);
            drawCardObjects.Add(cardObject);
        }
        bool isMine = playerID == 0;
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
        turnUI.onClick.AddListener(() => setAction());
    }

    public void UpdatePPUI(int playerID, int ppMax, int ppCurrent)
    {
        bool isMine = playerID == 0;
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
}
