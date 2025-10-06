using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandUI : BaseUI
{
    [SerializeField] private Transform handRoot = null;
    [SerializeField] private Transform drawRoot = null;
    private bool isAcssessible = false;
    private List<CardObject> handCards = new List<CardObject>();

    private const float HAND_SCALE_X = 6.5f;

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
    public async UniTask AddHandCard(List<CardObject> drawCard)
    {
        for (int i = 0, max = drawCard.Count; i < max; i++)
        {
            CardObject card = drawCard[i];
            handCards.Add(card);
            card.transform.SetParent(handRoot);
            card.SetCardState(CardObject.CardState.HAND);

            await card.DrawCard(drawRoot, handRoot);
        }
        // 手札の整列
        ArrangeHandCard();
    }

    // 手札エリアからカードを削除する
    public void RemoveHandCard(CardObject card)
    {
        handCards.Remove(card);

        ArrangeHandCard();
    }

    // 手札エリアにカードを整列させる
    public void ArrangeHandCard()
    {
        float areaWidth = HAND_SCALE_X;
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
            // 左端基準のX
            float xPosition = -totalWidth / 2 + actualWidth * i + actualWidth / 2;

            // Yは厚み分上げる
            float yPosition = cardThickness * i;

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
