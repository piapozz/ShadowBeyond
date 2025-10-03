using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

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
    [SerializeField] private RectTransform handField;
    [SerializeField] private TurnUI turnUI;
    [SerializeField] private DeckUI deckUI;
    [SerializeField] private FieldUI fieldUI;
    [SerializeField] private LeaderUI leaderUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private HistoryUI historyUI;
    [SerializeField] private PPUI ppUI;
    [SerializeField] private InfoUI infoUI;

    public static UIManager instance { get; private set; }

    private UniTaskCompletionSource _uniTaskCompletionSource = null;

    public override UniTask Initialize()
    {
        instance = this;
        // UIを生成
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);

        // それぞれのUIにコールバック設定


        return UniTask.CompletedTask;
    }

    private void Start()
    {
        MasterDataManager.LoadAllData();

        instance = this;
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);

        for (int i = 0; i < 4; i++)
        {
            var cardData = CardMasterUtility.GetCardMaster(1200);
            CardData.FollowerStatus status = new CardData.FollowerStatus(cardData.Attack, cardData.Defence);
            CardData setData = new CardData(
                cardData.ID,
                (GameEnum.LeaderClass)cardData.Class,
                (GameEnum.CardRarity)cardData.Rarity,
                (GameEnum.CardType)cardData.Type,
                cardData.Name,
                cardData.Cost,
                cardData.Attack,
                cardData.Defence);
            CardObject handObject = Instantiate(cardObject);
            handObject.SetCardData(setData);
            handUI.AddHandCard(handObject);
            CardObject fieldCard = Instantiate(cardObject);
            fieldCard.SetCardData(setData);
            fieldUI.AddOpponentFieldCard(fieldCard);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 情報UI更新
        // OptionUI更新
        // HistoryUI更新
    }

    // ターン開始
    public void StartTurn()
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
    public async UniTask<SendData> InputUI()
    {
        _uniTaskCompletionSource = new UniTaskCompletionSource();



        await _uniTaskCompletionSource.Task;
        return new SendData();
    }

    public void SetCardDrop(CardObject setCard)
    {
        Vector3 mousePos = Input.mousePosition;

        // 手札領域か判定
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(handField, mousePos);
        // 手札に戻す
        if (isInside)
        {
            handUI.ArrangeHandCard();
        }
        // カードをプレイ
        else
        {
            // 手札から除外しフィールドに追加
            handUI.RemoveHandCard(setCard);
            setCard.SetCardState(CardObject.CardState.FIELD);
            fieldUI.AddOwnFieldCard(setCard);
        }
    }
}
