using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Transform ownHandRoot = null;
    [SerializeField] private Transform opponentHandRoot = null;
    [SerializeField] private Transform ownDrawRoot = null;
    private List<CardObject> ownHandCards = new List<CardObject>();
    private List<CardObject> opponentHandCards = new List<CardObject>();

    private const float HAND_SCALE_X = 5.0f;

    // カードをドローする
    public void DrawCard(bool isMine, List<CardObject> drawCards, Transform deckRoot)
    {
        List<Sequence> sequenceList = new List<Sequence>();
        for (int i = 0, max = drawCards.Count; i < max; i++)
        {
            CardObject card = drawCards[i];
            card.SetCardState(CardObject.CardState.HAND);
            card.SetIsLocal(isMine);
            if (isMine)
            {
                ownHandCards.Add(card);
                // 手札を引くDOTweenのSequenceを取得
                sequenceList.Add(card.DrawOwnCard(deckRoot, ownDrawRoot, ownHandRoot));
            }
            else
            {
                opponentHandCards.Add(card);
                sequenceList.Add(card.DrawOpponentCard(deckRoot, opponentHandRoot));
            }
        }
        // ドローの挙動と整列の挙動を登録
        UIManager.instance.AddSequence(sequenceList);
        UIManager.instance.AddSequence(ArrangeHandCard(isMine));
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
    public List<Sequence> ArrangeHandCard(bool isMine)
    {
        float areaWidth = HAND_SCALE_X;
        float cardWidth = 1.0f;          // 仮のカード幅
        float cardThickness = 0.15f;     // カードの厚み（Y方向のずらし幅）

        List<CardObject> cardList = null;
        if (isMine) cardList = ownHandCards;
        else cardList = opponentHandCards;
        int cardCount = cardList.Count;

        if (cardCount == 0) return null;

        List<Sequence> sequenceList = new List<Sequence>();

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

            Sequence arrangeHandSeq = DOTween.Sequence();
            arrangeHandSeq.Append(cardList[i].transform.DOLocalMove(new Vector3(xPosition, yPosition, 0), 0.5f));
            sequenceList.Add(arrangeHandSeq);
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
