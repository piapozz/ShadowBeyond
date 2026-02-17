using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [SerializeField]
    private TMP_InputField SearchInputField;

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

        cardDatas.Clear();
        cardDatas = CardMasterUtility.allCardList;

        // トークンカードを除外
        cardDatas.RemoveAll(card =>
        {
            var data = CardMasterUtility.GetCardData(card.id, false);
            return data.isToken == true;
        }
        );
        SortCardList();
        maxPage = (cardDatas.Count - 1) / CardsPerPage;
        UpdateCardList();
    }

    // 更新処理
    public void UpdateCardList()
    {
        var cardData = CardMasterUtility.GetCardData(currentPage, false);

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

    // 条件更新
    public void UpdateConditions()
    {
        SortCradData();
        currentPage = 0;
        UpdateCardList();
    }

    // 現在の検索設定に基づいてデータを更新
    private void SortCradData()
    {
        cardDatas = new List<CardData>(CardMasterUtility.allCardList);

        // トークンカードを除外
        cardDatas.RemoveAll(card => 
            {
                var data = CardMasterUtility.GetCardData(card.id, false);
                return data.isToken == true;
            }
        );

        // 条件に基づいてフィルタリング
        // コスト条件
        // 1,2,3,4,5,6,7,8,9,10+ 
        List<bool> costConditions = ConditionManager.Instance.GetCostCondition();
        cardDatas.RemoveAll(card => 
        {
            int cost = card.cost;
            if (cost >= 10) cost = 10;
            return costConditions[cost] == false;
        });

        // カードタイプ条件
        List<bool> typeConditions = ConditionManager.Instance.GetTypeCondition();
        cardDatas.RemoveAll(card =>
        {
            int typeIndex = (int)card.type;
            return typeConditions[typeIndex] == false;
        });

        // リーダー条件
        List<bool> leaderConditions = ConditionManager.Instance.GetLeaderCondition();
        cardDatas.RemoveAll(card =>
        {
            int leaderIndex = (int)card.leaderClass;
            return leaderConditions[leaderIndex] == false;
        });

        // レアリティ条件
        List<bool> rarityConditions = ConditionManager.Instance.GetRarityCondition();
        cardDatas.RemoveAll(card =>
        {
            int rarityIndex = (int)card.rarity;
            return rarityConditions[rarityIndex] == false;
        });

        // パック条件
        List<bool> packConditions = ConditionManager.Instance.GetPackCondition();
        cardDatas.RemoveAll(card =>
        {
            int packIndex = (int)card.packType;
            return packConditions[packIndex] == false;
        });

        SortCardList();

        maxPage = (cardDatas.Count - 1) / CardsPerPage;
        currentPage = 0;
    }

    // 文字列条件
    public void StringCondition()
    {
        cardDatas = new List<CardData>(CardMasterUtility.allCardList);

        // トークンカードを除外
        cardDatas.RemoveAll(card =>
        {
            var data = CardMasterUtility.GetCardData(card.id, false);
            return data.isToken == true;
        }
        );

        string condition = SearchInputField.text;

        if (!string.IsNullOrEmpty(condition))
        {
            string[] keywords = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            cardDatas.RemoveAll(card =>
            {
                string name = card.name ?? "";
                string text = card.text ?? "";

                // すべてのキーワードが name または text のどちらかに含まれているか
                return !keywords.All(k =>
                    name.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                    text.Contains(k, StringComparison.OrdinalIgnoreCase));
            });
        }

        SortCardList();

        maxPage = (cardDatas.Count - 1) / CardsPerPage;
        currentPage = 0;
    }

    // リストの並びに順を揃える
    public void SortCardList()
    {
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

        UpdateCardList();
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
