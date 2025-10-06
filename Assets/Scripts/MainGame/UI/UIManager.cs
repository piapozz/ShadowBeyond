using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static NetWorkModule;

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
    [SerializeField] private GameObject deckObject;

    public static UIManager instance { get; private set; }

    private UniTaskCompletionSource _uniTaskCompletionSource = null;

    private List<CardObject> poolCardObject = null;
    private Transform deckTransform = null;

    private const int POOL_CARD_NUM = 30;

    public override async UniTask Initialize()
    {
        MasterDataManager.LoadAllData();
        instance = this;
        // UIを生成
        handUI = Instantiate(handUI);
        fieldUI = Instantiate(fieldUI);
        deckTransform = Instantiate(deckObject).transform;

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

    // Update is called once per frame
    void Update()
    {
        // 情報UI更新
        // OptionUI更新
        // HistoryUI更新
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
    public async UniTask<SendData> InputUI()
    {
        _uniTaskCompletionSource = new UniTaskCompletionSource();



        await _uniTaskCompletionSource.Task;
        return new SendData();
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
    /// カードがドロップされたときの処理
    /// </summary>
    /// <param name="setCard"></param>
    public void DropCard(CardObject setCard)
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
            switch (setCard.cardData.type)
            {
                case GameEnum.CardType.FOLLOWER:
                case GameEnum.CardType.AMULET:
                    fieldUI.AddOwnFieldCard(setCard);
                    break;
                case GameEnum.CardType.SPELL:
                    setCard.SetCardState(CardObject.CardState.UNUSE);
                    break;
                default: break;
            }
        }
    }

    /// <summary>
    /// デッキからカードをドローする
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="drawCard"></param>
    public async UniTask DrawCards(int playerID, List<CardData> drawCard)
    {
        int drawCardNum = drawCard.Count;
        List<CardObject> drawCardObjects = new List<CardObject>(drawCardNum);
        for (int i = 0; i < drawCardNum; i++)
        {
            CardObject cardObject = GetUnuseCardObject();
            // オブジェクトの座標と回転をセット
            Transform cardTransform = cardObject.transform;
            cardTransform.position = deckTransform.position;
            cardTransform.rotation = deckTransform.rotation;

            cardObject.gameObject.SetActive(true);
            // カードデータセット
            cardObject.SetCardData(drawCard[0]);
            cardObject.SetCardState(CardObject.CardState.HAND);
            drawCardObjects.Add(cardObject);
        }
        await handUI.AddHandCard(drawCardObjects);
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
}
