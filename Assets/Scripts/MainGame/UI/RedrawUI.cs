using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

public class RedrawUI : MonoBehaviour
{
    [SerializeField]
    private List<Transform> keepRoot;

    [SerializeField]
    private List<Transform> redrawRoot;
 
    [SerializeField]
    private Button redrawButton;

    [SerializeField]
    private RectTransform redrawArea;

    private Transform deckRoot;
    List<CardData> keepCard;
    List<CardData> redrawCard;

    bool isRedraw = false;

    public void StartRedraw(List<CardData> cards, Transform setDeckRoot)
    {
        // リドロー用リストの初期化
        keepCard = new List<CardData>(new CardData[4]);
        redrawCard = new List<CardData>(new CardData[4]);

        deckRoot = setDeckRoot;
        isRedraw = true;

        List<Sequence> drawSequences = new List<Sequence>();

        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var cardObj = card.GetObject();
            var root = keepRoot[i];

            // カード初期設定
            cardObj.SetCardState(CardObject.CardState.REDRAW);
            cardObj.SetIsLocal(true);
            keepCard[i] = card;

            // 初期位置・回転
            cardObj.transform.position = deckRoot.position;
            cardObj.transform.rotation = deckRoot.rotation;

            Sequence drawSeq = DOTween.Sequence();

            drawSeq
                .AppendCallback(() => cardObj.gameObject.SetActive(true))
                .Join(cardObj.transform.DOMove(root.position, 0.5f))
                .Join(cardObj.transform.DORotate(root.localEulerAngles, 0.5f));

            drawSequences.Add(drawSeq);
        }

        // まとめて登録
        UIManager.instance.AddSequence(drawSequences);

        // 相手も引く
        var opponent = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OPPONENT);
        opponent.deck.DrawDeck(cards.Count);

        // リドローボタンの設定
        redrawButton.onClick.RemoveAllListeners();
        redrawButton.onClick.AddListener(async () => await Redraw());
        var btnText = redrawButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = "決定";
    }

    public async UniTask Redraw()
    {
        var player = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OWN);

        // 戻したカードを引かないように戻す前にひく
        var redrawCount = redrawCard.FindAll(card => card != null).Count;
        List<CardData> cardDatas = player.deck.PeekDeck(redrawCount);
        for (int i = 0; i < cardDatas.Count; i++)
        {
            CardObject cardObject = UIManager.instance.GetUnuseCardObject();
            // カードデータセット
            cardObject.SetCardData(cardDatas[i]);
            cardObject.SetCardState(CardObject.CardState.REDRAW);
        }

        // 選択されたカードをデッキに戻す
        foreach (var card in redrawCard)
        {
            if (card == null) continue;
            player.deck.AddCard(card);
        }

        foreach (var card in cardDatas)
        {
            if (card == null) continue;
            var index = keepCard.IndexOf(null);
            keepCard[index] = card;
            card.GetObject().SetCardState(CardObject.CardState.REDRAW);
        }

        // キープカードを手札に追加
        foreach (var card in keepCard)
        {
            if (card == null) continue;
            player.hand.AddCard(card);
        }

        // 引き直すカード
        Debug.Log($"[Battle] 🔄 引き直すカードリスト: {string.Join(", ", redrawCard.ConvertAll(card => card != null ? card.name : "null"))}");
        // 引くカード
        Debug.Log($"[Battle] 🔄 引くカードリスト: {string.Join(", ", cardDatas.ConvertAll(card => card != null ? card.name : "null"))}");
        // 手札カードリスト
        Debug.Log($"[Battle] 🃏 手札カードリスト: {string.Join(", ", keepCard.ConvertAll(card => card != null ? card.name : "null"))}");

        // デッキに戻す動き
        List<Sequence> redrawSequence = new List<Sequence>();
        foreach (var card in redrawCard)
        {
            if (card == null) continue;
            var cardObj = card.GetObject();
            var root = deckRoot;
            Sequence drawSeq = DOTween.Sequence();
            drawSeq.Join(cardObj.transform.DOMove(root.position, 0.5f))
                .Join(cardObj.transform.DORotate(root.localEulerAngles , 0.5f))
                .AppendCallback(() => cardObj.SetCardState(CardObject.CardState.UNUSE));
            redrawSequence.Add(drawSeq);
        }

        // デッキからひく動き
        foreach (var card in cardDatas)
        {
            if (card == null) continue;
            var cardObj = card.GetObject();
            var index = keepCard.IndexOf(card);
            var root = keepRoot[index];
            Sequence drawSeq = DOTween.Sequence();
            drawSeq.AppendCallback(() => cardObj.transform.position = deckRoot.position)
                .AppendCallback(() => cardObj.transform.rotation = deckRoot.rotation)
                .AppendCallback(() => cardObj.gameObject.SetActive(true))
                .Join(cardObj.transform.DOMove(root.position, 0.5f))
                .Join(cardObj.transform.DORotate(root.localEulerAngles , 0.5f));
            redrawSequence.Add(drawSeq);
        }
        UIManager.instance.AddSequence(redrawSequence);

        // ボタン
        redrawButton.onClick.RemoveAllListeners();
        redrawButton.enabled = false;
        var btnText = redrawButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = "引き直し中...";
        await UIManager.instance.IsCompleteAllSequenceTask();

        await EndRedraw();
    }

    public async UniTask EndRedraw()
    {
        // マリガン送信
        List<bool> isRedrawList = redrawCard.ConvertAll(card => card != null);
        if (isRedrawList == null || isRedrawList.Count == 0)
        {
            Debug.LogError("[Battle] ❌ マリガンが不正です");
            return;
        }

        NetworkManager.Instance.SendRedrawData(isRedrawList);
        Debug.Log("[Battle] 📤 マリガン送信完了");

        // 相手マリガン受信待機
        Debug.Log("[Battle] 📥 相手のマリガン受信待機中...");
        await BattleManager.instance.WaitUntil(() => NetworkManager.Instance.redrawReceived);
        var opponentIsRedraw = NetworkManager.Instance.GetReceivedRedrawList();
        Debug.Log($"[Battle] ✅ 相手マリガン受信完了");

        // UIを非表示
        this.gameObject.SetActive(false);
        var player = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OWN);
        var playerHand = player.hand.GetCards((card)=> null != card);
        // カードを手札の位置に移動
        UIManager.instance.AddHandCard((int)GameEnum.PlayerType.OWN, playerHand);
        // 手札カードリスト
        Debug.Log($"[Battle] 🃏 手札カードリスト: {string.Join(", ", playerHand.ConvertAll(card => card != null ? card.name : "null"))}");

        // 相手のマリガン処理
        var opponent = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OPPONENT);
        int opponentRedrawCount = opponentIsRedraw.FindAll(isRedraw => isRedraw).Count;
        List<CardData> cardList = opponent.deck.PeekDeck(opponentRedrawCount);
        for (int i = 0; i < opponentIsRedraw.Count; i++)
        {
            if (!opponentIsRedraw[i]) continue;
            var card = opponent.hand.GetCardAt(i);
            opponent.hand.ReturnCardToDeck(card);
            var cardData = cardList[0];
            cardList.RemoveAt(0);
            opponent.hand.InsertCardAt(cardData, i);
            cardData.GetObject().SetCardState(CardObject.CardState.HAND);
            var cardObject = cardData.GetObject().GetCardObject()[(int)CardObject.CardState.HAND];
            CardLook cardLook = cardObject.GetComponent<CardLook>();
            cardLook.SetCardFrontActive(false);
        }


        await UIManager.instance.IsCompleteAllSequenceTask();
        // 相手の手札
        var opponentHand = opponent.hand;
        // 手札のステート切り替え
        foreach(var card in opponentHand.GetCards((card) => null != card))
        {
            card.GetObject().SetCardState(CardObject.CardState.HAND);
            var cardObject = card.GetObject().GetCardObject()[(int)CardObject.CardState.HAND];
            CardLook cardLook = cardObject.GetComponent<CardLook>();
            cardLook.SetCardFrontActive(false);
        }
        Debug.Log($"[Battle] 🃏 相手手札カードリスト: {string.Join(", ", opponentHand.GetCards((card) => null != card).ConvertAll(card => card != null ? card.name : "null"))}");
        // デッキシャッフル
        player.deck.ShuffleDeck();
        opponent.deck.ShuffleDeck();

        isRedraw = false;
    }

    // リドローエリア内かどうか
    public async UniTask IsInRedrawArea(CardObject card)
    {
        Vector3 mousePos = Input.mousePosition;

        // フィールド領域か判定
        bool isField = RectTransformUtility.RectangleContainsScreenPoint(
            redrawArea,
            mousePos,
            Camera.main
        );
        // マリガンリストに移動
        if (isField)
        {
            var cardData = card.cardData;
            int index = keepCard.IndexOf(cardData);
            if (index < 0) index = redrawCard.IndexOf(cardData);
            keepCard[index] = null;
            var root = redrawRoot[index];
            // アニメーションで移動
            Sequence moveSeq = DOTween.Sequence();
            moveSeq.AppendCallback(() => card.transform.position = card.transform.position)
                .AppendCallback(() => card.transform.rotation = card.transform.rotation)
                .Join(card.transform.DOMove(root.position, 0.5f))
                .Join(card.transform.DORotate(root.localEulerAngles , 0.5f));
            UIManager.instance.AddSequence(new List<Sequence>() { moveSeq });
            redrawCard[index] = cardData;

        }
        // もとに戻す
        else
        {
            var cardData = card.cardData;
            int index = keepCard.IndexOf(cardData);
            if (index < 0) index = redrawCard.IndexOf(cardData);
            redrawCard[index] = null;
            var root = keepRoot[index];
            Sequence moveSeq = DOTween.Sequence();
            moveSeq.AppendCallback(() => card.transform.position = card.transform.position)
                .AppendCallback(() => card.transform.rotation = card.transform.rotation)
                .Join(card.transform.DOMove(root.position, 0.5f))
                .Join(card.transform.DORotate(root.localEulerAngles, 0.5f));
            UIManager.instance.AddSequence(new List<Sequence>() { moveSeq });
            keepCard[index] = cardData;
        }

        while (!UIManager.instance.IsCompleteAllSequence())
        {
            await UniTask.Yield();
        }
    }

    public bool IsRedraw()
    {
        return isRedraw;
    }
}
