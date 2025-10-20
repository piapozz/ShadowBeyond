using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FieldUI : MonoBehaviour
{
    private List<CardObject> ownCards = new List<CardObject>();
    private List<CardObject> opponentCards = new List<CardObject>();

    [SerializeField] private Transform ownFieldRoot = null;
    [SerializeField] private Transform opponentFieldRoot = null;
    [SerializeField] private Transform playCardRoot = null;
    [SerializeField] private List<Transform> fieldCardSlotList = null;

    private const float FIELD_SCALE_X = 10.0f;
    private const float FIELD_CARD_SPACE = 2.0f;

    public void AddOwnFieldCard(CardObject addCard)
    {
        ownCards.Add(addCard);
        addCard.transform.SetParent(ownFieldRoot);
        addCard.SetCardState(CardObject.CardState.FIELD);
        ArrangeFieldCard(true);
    }

    public void RemoveOwnFieldCard(CardObject removeCard)
    {
        ownCards.Remove(removeCard);
        ArrangeFieldCard(true);
    }

    public void AddOpponentFieldCard(CardObject addCard)
    {
        opponentCards.Add(addCard);
        addCard.transform.SetParent(opponentFieldRoot);
        addCard.SetCardState(CardObject.CardState.FIELD);
        ArrangeFieldCard(false);
    }

    public void RemoveOpponentFieldCard(CardObject removeCard)
    {
        opponentCards.Remove(removeCard);
        ArrangeFieldCard(false);
    }

    private void ArrangeFieldCard(bool isOwn)
    {
        List<CardObject> fieldCards = null;
        if (isOwn)
            fieldCards = ownCards;
        else
            fieldCards = opponentCards;
            float areaWidth = FIELD_SCALE_X;
        float cardWidth = FIELD_CARD_SPACE;          // 仮のカード幅
        int cardCount = fieldCards.Count;

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

            fieldCards[i].transform.localPosition = new Vector3(xPosition, 0, 0);
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

    public Transform GetPlayCardRoot()
    {
        return playCardRoot;
    }
}
