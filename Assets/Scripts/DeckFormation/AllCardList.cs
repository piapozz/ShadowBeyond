using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardObject;

// マスターデータからカードリストを取得して表示する
// 1ページに8枚表示、ページ送り対応

public class AllCardList : MonoBehaviour
{
    private const int CardsPerPage = 8;

    private int currentPage = 0;
    private int maxPage = 0;

    [SerializeField]
    private RectTransform CardListArea;

    [SerializeField]
    private CardImage CardImage;

    private List<Transform> cardPositions = new List<Transform>();
    private List<GameObject> cardObjects = new List<GameObject>();
    private List<CardData> cardDatas = new List<CardData>();

    // Start is called before the first frame update
    void Start()
    {
        // マスターデータをロード
        MasterDataManager.LoadAllData();

        // カードエリアを3D空間に変換
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(CardListArea.position);
        float width = CardListArea.rect.width * CardListArea.lossyScale.x;
        float height = CardListArea.rect.height * CardListArea.lossyScale.y;
        float depth = 5.0f; // 適当な深度を設定
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(CardListArea.position.x - width / 2, CardListArea.position.y - height / 2, depth));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(CardListArea.position.x + width / 2, CardListArea.position.y - height / 2, depth));
        float convertedWidth = Vector3.Distance(bottomLeft, bottomRight);

        // 変換された幅を8等分してカードの位置を設定

        for (int i = 0; i < 8; i++)
        {
            float xPos = bottomLeft.x + (convertedWidth / 8) * (i + 0.5f);
            Vector3 cardPos = new Vector3(xPos, worldPos.y, worldPos.z + depth);
            GameObject cardPositionObj = new GameObject("CardPosition" + i);
            cardPositionObj.transform.position = cardPos;
            cardPositions.Add(cardPositionObj.transform);

            // カードイメージを生成して初期化
            GameObject cardObj = Instantiate(CardImage.gameObject, cardPos, Quaternion.identity);
            cardObj.transform.SetParent(this.transform);
            cardObjects.Add(cardObj);
        }

        SortCradData();
        UpdateCardList();
    }

    // 更新処理
    public void UpdateCardList()
    {
        var cardData = CardMasterUtility.GetCardData(currentPage);

        for(int i = currentPage * CardsPerPage; i < (currentPage + 1) * CardsPerPage; i++)
        {
            int cardIndex = i - currentPage * CardsPerPage;
            if (i < cardDatas.Count)
            {
                cardObjects[cardIndex].SetActive(true);
                var cardImage = cardObjects[cardIndex].GetComponent<CardImage>();
                cardImage.SetCardImage(cardDatas[i].id);
                cardObjects[cardIndex].transform.position = cardPositions[cardIndex].position;
            }
            else
            {
                cardObjects[cardIndex].SetActive(false);
            }
        }
    }

    // 現在の検索設定に基づいてデータを更新
    private void SortCradData()
    {
        cardDatas.Clear();
        cardDatas = CardMasterUtility.allCardList;

        // トークンカードを除外
        cardDatas.RemoveAll(card => 
            {
                var data = CardMasterUtility.GetCardData(card.id);
                return data.isToken == true;
            }
        );

        // コスト順 > リーダー順 > カードタイプ順　> レアリティ順 > 名前順でソート
        cardDatas.Sort((a, b) =>
        {
            int costComparison = a.cost.CompareTo(b.cost);
            if (costComparison != 0) return costComparison;
            int leaderComparison = a.leaderClass.CompareTo(b.leaderClass);
            if (leaderComparison != 0) return leaderComparison;
            int typeComparison = a.type.CompareTo(b.type);
            if (typeComparison != 0) return typeComparison;
            int rarityComparison = a.rarity.CompareTo(b.rarity);
            if (rarityComparison != 0) return rarityComparison;
            return a.name.CompareTo(b.name);
        });

        maxPage = (cardDatas.Count - 1) / CardsPerPage;
    }

    public void NextPage()
    {
        currentPage++;
        if (currentPage > maxPage)
        {
            currentPage = 0;
        }
        // カードリストの更新処理
        UpdateCardList();
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
        }
        else
        {
            currentPage = maxPage;
        }
        
        // カードリストの更新処理
        UpdateCardList();
    }
}
