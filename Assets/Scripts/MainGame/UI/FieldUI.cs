using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldUI : MonoBehaviour
{
    private List<CardObject> ownCards = new List<CardObject>();
    private List<CardObject> opponentCards = new List<CardObject>();

    [SerializeField] private Transform ownFieldRoot = null;
    [SerializeField] private Transform opponentFieldRoot = null;

    private const float FIELD_SCALE_X = 10.0f;
    private const float FIELD_CARD_SPACE = 2.0f;

    public void AddOwnFieldCard(CardObject addCard)
    {
        ownCards.Add(addCard);
        Debug.Log(addCard.cardData.name);
        addCard.transform.SetParent(ownFieldRoot);
        addCard.SetCardState(CardObject.CardState.FIELD);
        ArrangeOwnFieldCard();
    }

    public void RemoveOwnFieldCard(CardObject removeCard)
    {
        ownCards.Remove(removeCard);
        ArrangeOwnFieldCard();
    }

    public void AddOpponentFieldCard(CardObject addCard)
    {
        opponentCards.Add(addCard);
        Debug.Log(addCard.cardData.name);
        addCard.transform.SetParent(opponentFieldRoot);
        addCard.SetCardState(CardObject.CardState.FIELD);
        ArrangeOpponentFieldCard();
    }

    public void RemoveOpponentFieldCard(CardObject removeCard)
    {
        opponentCards.Remove(removeCard);
        ArrangeOpponentFieldCard();
    }

    private void ArrangeOwnFieldCard()
    {
        float areaWidth = FIELD_SCALE_X;
        float cardWidth = FIELD_CARD_SPACE;          // 仮のカード幅
        int cardCount = ownCards.Count;

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

            ownCards[i].transform.localPosition = new Vector3(xPosition, 0, 0);
        }
    }

    public void ArrangeOpponentFieldCard()
    {
        float areaWidth = FIELD_SCALE_X;
        float cardWidth = FIELD_CARD_SPACE;          // 仮のカード幅
        int cardCount = opponentCards.Count;

        if (cardCount == 0) return;

        // 「通常の幅」と「エリア内に収めるための幅」を計算
        float maxCardWidth = areaWidth / cardCount;
        float actualWidth = Mathf.Min(cardWidth, maxCardWidth);

        // 全体の横幅を計算（中央揃え用）
        float totalWidth = actualWidth * cardCount;

        for (int i = 0; i < cardCount; i++)
        {
            // 左端基準のX
            float xPosition = totalWidth / 2 - actualWidth * i - actualWidth / 2;

            opponentCards[i].transform.localPosition = new Vector3(xPosition, 0, 0);
        }
    }

    public int GetOwnFieldIndex(CardObject cardObject)
    {
        if (cardObject == null) return -1;

        return ownCards.IndexOf(cardObject);
    }

    public int GetOpponentFieldIndex(CardObject cardObject)
    {
        if (cardObject == null) return -1;

        return opponentCards.IndexOf(cardObject);
    }
}
