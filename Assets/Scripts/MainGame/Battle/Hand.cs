using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

// 手札クラス
public class Hand 
{
    private List<CardData> handCardList;

    public const int MAX_HAND = 9;

    public int playerID { get; private set; } = -1;

    private Field field;
    private Deck deck;

    public void SetPlayerID(int index)
    {
        playerID = index;
        field = BattleManager.instance.field;
        deck = BattleManager.instance.GetPlayer(index).deck;
    }

    public void Init(List<CardData> initialCards)
    {
        handCardList = new List<CardData>(initialCards);
    }

    /// <summary>
    /// 指定カードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(CardData card)
    {
        if (card == null) return;
        // 今は制限なし
        //if (handCardList.Count >= MAX_HAND) return;
        handCardList.Add(card);
        // 手札のプレイ可否更新
        UpdatePlayableCards();
    }

    public void AddCards(List<CardData> cards)
    {
        for (int i = 0, max = cards.Count; i < max; i++)
        {
            if (cards[i] == null) return;
            handCardList.Add(cards[i]);
        }
        // 手札のプレイ可否更新
        UpdatePlayableCards();
    }

    /// <summary>
    /// 指定カードを指定番目に挿入
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    public void InsertCardAt(CardData card, int index)
    {
        if (card == null) return;
        if (handCardList.Count >= MAX_HAND) return;
        if (index < 0 || index > handCardList.Count)
        {
            handCardList.Add(card);
        }
        else
        {
            handCardList.Insert(index, card);
        }

        // 手札に加えるアニメーション
        UIManager.instance.InsertDrawCards(playerID, card, index);
    }

    /// <summary>
    /// 指定カードを出す
    /// </summary>
    /// <param name="card"></param>
    public void PlayCard(CardData card, bool toField)
    {
        // プレイ可否更新
        card.SetCanPlay(false);
        handCardList.Remove(card);
        if (toField)
            field.PlayCard(card, playerID);
    }

    /// <summary>
    /// 指定カードをデッキに戻す
    /// </summary>
    /// <param name="card"></param>
    public void ReturnCardToDeck(CardData card)
    {
        if (card == null) return;
        handCardList.Remove(card);
        deck.AddCard(card);

        // デッキに戻すアニメーション
        UIManager.instance.ReturnCards(playerID, new List<CardData> { card });
    }

    /// <summary>
    /// 指定カードをデッキに戻す
    /// </summary>
    /// <param name="card"></param>
    public void ReturnCardToDeck(List<CardData> cardList)
    {
        if (cardList == null || cardList.Count == 0) return;
        for (int i = 0, max = cardList.Count; i < max; i++)
        {
            handCardList.Remove(cardList[i]);
            deck.AddCard(cardList[i]);
        }

        // デッキに戻すアニメーション
        UIManager.instance.ReturnCards(playerID, cardList);
    }

    /// <summary>
    /// 指定番目のカードを取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CardData GetCardAt(int index)
    {
        if (index < 0 || index >= handCardList.Count) return null;
        return handCardList[index];
    }

    /// <summary>
    /// 条件に合うカードをすべて取得
    /// </summary>
    public List<CardData> GetCards(Func<CardData, bool> condition)
    {
        return handCardList.Where(condition).ToList();
    }

    /// <summary>
    /// 条件に合うカードを1枚取得（ランダム）
    /// </summary>
    public CardData GetRandomCard(Func<CardData, bool> condition)
    {
        var matches = GetCards(condition);
        if (matches.Count == 0) return null;
        return matches[BattleManager.instance.rand.Next(0, matches.Count)];
    }

    /// <summary>
    /// 条件に合うカードをすべて除外
    /// </summary>
    public void RemoveCards(Func<CardData, bool> condition)
    {
        handCardList.RemoveAll(c => condition(c));
    }

    public void RemoveCard(CardData removeCard)
    {
        handCardList.Remove(removeCard);
    }

    public void RemoveCards(List<CardData> removeCards)
    {
        for (int i = 0, max = removeCards.Count; i < max; i++)
        {
            handCardList.Remove(removeCards[i]);
        }
    }

    /// <summary>
    /// 手札枚数を取得
    /// </summary>
    public int GetHandCount()
    {
        return handCardList.Count;
    }

    /// <summary>
    /// 手札の最小コストのカードを取得
    /// </summary>
    public CardData GetMinCostCard()
    {
        return handCardList.OrderBy(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// 手札の最大コストのカードを取得
    /// </summary>
    public CardData GetMaxCostCard()
    {
        return handCardList.OrderByDescending(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// 手札のプレイ可否更新
    /// </summary>
    public void UpdatePlayableCards()
    {
        // 自分のターンであればコストを参照し更新
        if (BattleManager.instance.currentPlayerIndex != (int)GameEnum.PlayerType.OWN) return;
        var leader = BattleManager.instance.GetPlayer((int)GameEnum.PlayerType.OWN).leader;
        for (int i = 0, max = handCardList.Count; i < max; i++)
        {
            int playableCost = handCardList[i].GetPlayableCost(leader.currentPlayPoint);
            if (playableCost < 0)
            {
                handCardList[i].SetCanPlay(false);
            }
            else
            {
                handCardList[i].SetCanPlay(true);
            }
        }
    }

    /// <summary>
    /// 手札のすべてのカードのプレイ可否を設定
    /// </summary>
    /// <param name="playable"></param>
    public void SetOwnHandCardPlayable(bool playable)
    {
        for (int i = 0, max = handCardList.Count; i < max; i++)
        {
            handCardList[i].SetCanPlay(playable);
        }
    }

    public List<CardData> GetCards(TargetCondition condition)
    {
        return BattleManager.instance.GetCards(handCardList, condition);
    }
}
