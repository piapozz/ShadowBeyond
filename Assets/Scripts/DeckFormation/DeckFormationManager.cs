using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckFormationManager : MonoBehaviour
{

    [SerializeField]
    private CardImage CardImage;

    [SerializeField]
    private RectTransform DeckArea;

    [SerializeField]
    private Scrollbar Scrollbar;

    public static DeckFormationManager Instance;

    private List<CardImage> cardDataList = null; 

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        cardDataList = new List<CardImage>();
    }

    public void AddCardToDeck(int cardId)
    {
        // デッキにカードを追加する処理
        var cardData = CardMasterUtility.GetCardData(cardId);

        var card = Instantiate(CardImage.gameObject, DeckArea);
        var cardImage = card.GetComponent<CardImage>();
        cardImage.SetCardImage(cardId);
        cardDataList.Add(cardImage);

        UpdateCardList();
    }

    public void RemoveCardFromDeck(int cardId)
    {
        // デッキからカードを削除する処理
        for (int i = 0; i < cardDataList.Count; i++)
        {
            var cardImage = cardDataList[i].GetComponent<CardImage>();
            if (cardImage.cardId == cardId)
            {
                Destroy(cardDataList[i]);
                cardDataList.RemoveAt(i);
                break;
            }
        }
        UpdateCardList();
    }

    public void UpdateCardList()
    {
        SortCradData();
     
        // デッキリストの更新処理
        foreach (var card in cardDataList)
        {
            card.gameObject.SetActive(false);
        }

        // エリアを3D空間に変換
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(DeckArea.position);
        float width = DeckArea.rect.width * DeckArea.lossyScale.x;
        float height = DeckArea.rect.height * DeckArea.lossyScale.y;
        float depth = 5.0f; // DeckArea
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(DeckArea.position.x - width / 2, DeckArea.position.y - height / 2, depth));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(DeckArea.position.x + width / 2, DeckArea.position.y - height / 2, depth));
        float convertedWidth = Vector3.Distance(bottomLeft, bottomRight);
        // 8等分してカードの位置を設定
        // スクロールバーの値に基づいて開始インデックスを計算
        // 表示するカード数を8枚に制限
        int visibleCardCount = 8;
        int startIndex = Mathf.FloorToInt(Scrollbar.value * Mathf.Max(0, cardDataList.Count - visibleCardCount));
        for (int i = 0; i < visibleCardCount; i++)
        {
            int cardIndex = startIndex + i;
            if (cardIndex < cardDataList.Count)
            {
                float xPos = bottomLeft.x + (convertedWidth / visibleCardCount) * (i + 0.5f);
                Vector3 cardPos = new Vector3(xPos, worldPos.y, worldPos.z + depth);
                cardDataList[cardIndex].transform.position = cardPos;
                cardDataList[cardIndex].gameObject.SetActive(true);
            }
            else
            {
                // 表示するカードがない場合は非表示にする
                if (i < cardDataList.Count)
                {
                    cardDataList[i].gameObject.SetActive(false);
                }
            }
        }

        // スクロールバーの最大値を更新
        float maxScrollbarValue = Mathf.Max(0, cardDataList.Count - visibleCardCount);
        Scrollbar.size = visibleCardCount / (float)cardDataList.Count;
    }

    // 現在の検索設定に基づいてデータを更新
    private void SortCradData()
    {
        // コスト順 > リーダー順 > カードタイプ順　> レアリティ順 > 名前順でソート
        cardDataList.Sort((a, b) =>
        {
            var aData = CardMasterUtility.GetCardData(a.cardId);
            var bData = CardMasterUtility.GetCardData(b.cardId);
            int costComparison = aData.cost.CompareTo(bData.cost);
            if (costComparison != 0) return costComparison;
            int leaderComparison = aData.leaderClass.CompareTo(bData.leaderClass);
            if (leaderComparison != 0) return leaderComparison;
            int typeComparison = aData.type.CompareTo(bData.type);
            if (typeComparison != 0) return typeComparison;
            int rarityComparison = aData.rarity.CompareTo(bData.rarity);
            if (rarityComparison != 0) return rarityComparison;
            return a.name.CompareTo(b.name);
        });
    }
}
