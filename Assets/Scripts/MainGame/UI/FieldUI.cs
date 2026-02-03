using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

public class FieldUI : MonoBehaviour
{
    private List<CardObject> ownCards = new List<CardObject>();
    private List<CardObject> opponentCards = new List<CardObject>();

    [SerializeField] private Transform ownFieldRoot = null;
    [SerializeField] private Transform opponentFieldRoot = null;
    [SerializeField] private List<Transform> fieldCardSlotList = null;

    private const float FIELD_SCALE_X = 10.0f;
    private const float FIELD_CARD_SPACE = 2.0f;

    public void RemoveOwnFieldCard(CardObject removeCard)
    {
        ownCards.Remove(removeCard);
        UIManager.instance.AddSequence(ArrangeFieldCard(true));
    }

    public void RemoveOpponentFieldCard(CardObject removeCard)
    {
        opponentCards.Remove(removeCard);
        UIManager.instance.AddSequence(ArrangeFieldCard(false));
    }

    private List<Sequence> ArrangeFieldCard(bool isOwn, int addCardNum = 0)
    {
        List<CardObject> fieldCards = isOwn ? ownCards : opponentCards;
        float areaWidth = FIELD_SCALE_X;
        float cardWidth = FIELD_CARD_SPACE;          // 仮のカード幅
        int beforeCardNum = fieldCards.Count;
        int afterCardNum = beforeCardNum + addCardNum;

        if (afterCardNum == 0) return null;

        // 「通常の幅」と「エリア内に収めるための幅」を計算
        float maxCardWidth = areaWidth / afterCardNum;
        float actualWidth = Mathf.Min(cardWidth, maxCardWidth);

        // 全体の横幅を計算（中央揃え用）
        float totalWidth = actualWidth * afterCardNum;

        List<Sequence> sequenceList = new List<Sequence>();
        for (int i = 0; i < afterCardNum; i++)
        {
            // 左端基準のX
            float xPosition = -totalWidth / 2 + actualWidth * i + actualWidth / 2;
            // 相手の場なら逆順
            if (!isOwn)
                xPosition = -xPosition;

            if (i < beforeCardNum)
            {
                Sequence arrangeSequence = DOTween.Sequence();
                arrangeSequence.Append(fieldCards[i].transform.DOLocalMoveX(xPosition, 0.3f));
                sequenceList.Add(arrangeSequence);
            }
            else
            {
                Transform slotTransform = fieldCardSlotList[i - beforeCardNum];
                Transform targetParent = isOwn ? ownFieldRoot : opponentFieldRoot;
                slotTransform.SetParent(targetParent);
                slotTransform.localPosition = new Vector3(xPosition, 0, 0);
            }
        }
        return sequenceList;
    }

    public void PlayFieldCard(bool isOwn, CardObject playCard)
    {
        // 先に場を整列させる
        List<Sequence> playSequence = new List<Sequence>();
        playSequence.AddRange(ArrangeFieldCard(isOwn, 1));
        if (isOwn)
        {
            ownCards.Add(playCard);
            playSequence.Add(playCard.PlayFieldSequence(isOwn, fieldCardSlotList[0], ownFieldRoot));
        }
        else
        {
            opponentCards.Add(playCard);
            playSequence.Add(playCard.PlayFieldSequence(isOwn, fieldCardSlotList[0], opponentFieldRoot));
        }
        // プレイの挙動と整列の挙動を登録
        UIManager.instance.AddSequence(playSequence);
    }

    public void PlaySpellCard(bool isOwn, CardObject playCard)
    {
        playCard.PlaySpellCard(isOwn);
    }

    public void EnterFieldCard(bool isOwn, List<CardObject> enterCards)
    {
        UIManager.instance.AddSequence(ArrangeFieldCard(isOwn, enterCards.Count));
        for (int i = 0, max = enterCards.Count; i < max; i++)
        {
            if (isOwn)
                ownCards.Add(enterCards[i]);
            else
                opponentCards.Add(enterCards[i]);
            // 出したカードの座標設定
            enterCards[i].SetCardState(CardObject.CardState.FIELD);
            enterCards[i].transform.position = fieldCardSlotList[i].position;
            enterCards[i].SetIsLocal(isOwn);
        }
    }

    public void BounceCard(bool isOwn, List<CardObject> bounceCards)
    {
        for (int i = 0, max = bounceCards.Count; i < max; i++)
        {
            if (isOwn)
            {
                RemoveOwnFieldCard(bounceCards[i]);
            }
            else
            {
                RemoveOpponentFieldCard(bounceCards[i]);
            }
        };
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

    public CardObject GetOwnCard(int index)
    {
        return ownCards[index];
    }

    public CardObject GetOpponentCard(int index)
    {
        return opponentCards[index];
    }
}
