using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : BaseUI
{
    private bool isAcssessible = false;
    private List<CardUI> handCards = new List<CardUI>();

    [SerializeField] private BoxCollider handArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAcssessible) return;

        UpdateHandCard();
    }

    // アクセス可能にする
    public void SetAccessible(bool value)
    {
        isAcssessible = value;
    }

    // 手札エリアにカードを追加する
    public void AddHandCard(CardUI card)
    {
        handCards.Add(card);

        ArrangeHandCard();
    }

    // 手札エリアからカードを削除する
    public void RemoveHandCard(CardUI card)
    {
        handCards.Remove(card);

        ArrangeHandCard();
    }

    // 手札エリアにカードを整列させる
    public void ArrangeHandCard()
    {
        float areaWidth = handArea.size.x;
        float cardWidth = 1.0f;          // 仮のカード幅
        float cardThickness = 0.15f;     // カードの厚み（Y方向のずらし幅）
        int cardCount = handCards.Count;

        if (cardCount == 0) return;

        // 「通常の幅」と「エリア内に収めるための幅」を計算
        float maxCardWidth = areaWidth / cardCount;
        float actualWidth = Mathf.Min(cardWidth, maxCardWidth);

        // 全体の横幅を計算（中央揃え用）
        float totalWidth = actualWidth * cardCount;

        for (int i = 0; i < cardCount; i++)
        {
            // +Xから-Xに配置するので indexを反転
            int reversedIndex = cardCount - 1 - i;

            // 左端基準のX
            float xPosition = -totalWidth / 2 + actualWidth * reversedIndex + actualWidth / 2;

            // Yは厚み分下げる
            float yPosition = -cardThickness * i;

            handCards[i].transform.localPosition = new Vector3(xPosition, yPosition, 0);
        }
    }


    // 手札使用時のコールバック設定
    public void SetUseCardCallback(System.Action useCard)
    {

    }

    // 手札のカードを操作可能にする
    public void UpdateHandCard()
    {
        
    }
}
