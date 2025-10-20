using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Transform ownHandRoot = null;
    [SerializeField] private Transform opponentHandRoot = null;
    [SerializeField] private Transform ownDrawRoot = null;
    [SerializeField] private List<Transform> cardSlotList = null;
    private List<CardObject> ownHandCards = new List<CardObject>();
    private List<CardObject> opponentHandCards = new List<CardObject>();

    private const float HAND_SCALE_X = 5.0f;

    // カードをドローする
    public void DrawCard(bool isMine, List<CardObject> drawCards, Transform deckRoot)
    {
        // 先に手札を整列させ、ドローするカードの座標を取得
        List<Sequence> drawSequence = new List<Sequence>();
        int drawCardNum = drawCards.Count;
        drawSequence.AddRange(ArrangeHandCard(isMine, drawCardNum));
        // カードの諸設定をし、ドローするアニメーションを登録
        for (int i = 0; i < drawCardNum; i++)
        {
            CardObject card = drawCards[i];
            card.SetCardState(CardObject.CardState.HAND);
            card.SetIsLocal(isMine);
            if (isMine)
            {
                ownHandCards.Add(card);
                drawSequence.Add(card.DrawOwnCard(deckRoot, ownDrawRoot, cardSlotList[i], ownHandRoot));
            }
            else
            {
                opponentHandCards.Add(card);
                drawSequence.Add(card.DrawOpponentCard(deckRoot, cardSlotList[i], opponentHandRoot));
            }
        }
        // ドローの挙動と整列の挙動を登録
        UIManager.instance.AddSequence(drawSequence);
    }

    // 手札エリアからカードを削除する
    public void RemoveHandCard(bool isMine, CardObject card)
    {
        if (isMine)
        {
            ownHandCards.Remove(card);
        }
        else
        {
            opponentHandCards.Remove(card);
        }
        UIManager.instance.AddSequence(ArrangeHandCard(isMine));
    }

    /// <summary>
    /// オブジェクトからインデックスを取得
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int GetOwnCardIndex(CardObject card)
    {
        return ownHandCards.IndexOf(card);
    }

    /// <summary>
    /// インデックスからオブジェクトを取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CardObject GetOpponentCardObject(int index)
    {
        if (index < 0 || index >= opponentHandCards.Count) return null;
        return opponentHandCards[index];
    }

    /// <summary>
    /// 手札のカードを整列する
    /// </summary>
    /// <param name="isMine"></param>
    public List<Sequence> ArrangeHandCard(bool isMine, int addCardNum = 0)
    {
        float areaWidth = HAND_SCALE_X;
        float cardWidth = 1.0f;          // 仮のカード幅
        float cardThickness = 0.15f;     // カードの厚み（Y方向のずらし幅）

        List<CardObject> cardList = isMine ? ownHandCards : opponentHandCards;
        int beforeCardNum = cardList.Count;
        int afterCardNum = beforeCardNum + addCardNum;

        List<Sequence> sequenceList = new List<Sequence>();

        // 「通常の幅」と「エリア内に収めるための幅」を計算
        float maxCardWidth = areaWidth / afterCardNum;
        float actualWidth = Mathf.Min(cardWidth, maxCardWidth);

        // 全体の横幅を計算（中央揃え用）
        float totalWidth = actualWidth * afterCardNum;

        for (int i = 0; i < afterCardNum; i++)
        {
            // 左端基準のX
            float xPosition = -totalWidth / 2 + actualWidth * i + actualWidth / 2;

            // Yは厚み分上げる
            float yPosition = cardThickness * i;

            if (i < beforeCardNum)
            {
                Sequence arrangeHandSeq = DOTween.Sequence();
                arrangeHandSeq.Append(cardList[i].transform.DOLocalMove(new Vector3(xPosition, yPosition, 0), 0.5f));
                sequenceList.Add(arrangeHandSeq);
            }
            else
            {
                Transform slotTransform = cardSlotList[i - beforeCardNum];
                Transform targetParent;
                if (isMine)
                    targetParent = ownHandRoot;
                else
                    targetParent = opponentHandRoot;

                slotTransform.SetParent(targetParent);
                slotTransform.localPosition = new Vector3(xPosition, yPosition, 0);
            }
        }
        return sequenceList;
    }

    /// <summary>
    /// オブジェクト参照の自身の手札のインデックスを取得
    /// </summary>
    /// <param name="cardObject"></param>
    /// <returns></returns>
    public int GetOwnHandIndex(CardObject cardObject)
    {
        for (int i = 0, max = ownHandCards.Count; i < max; i++)
        {
            if (ownHandCards[i] == cardObject) return i;
        }
        return -1;
    }
}
