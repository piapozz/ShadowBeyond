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

    private int playerID = 0;

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
        if (handCardList.Count >= MAX_HAND) return;
        handCardList.Add(card);
    }

    /// <summary>
    /// 指定カードを場に出す
    /// </summary>
    /// <param name="card"></param>
    public void PlayCardToField(CardData card)
    {
        if (card == null) return;
        handCardList.Remove(card);
        // スペルならここで能力発動
        if (card.type == GameEnum.CardType.SPELL)
        {

        }

        field.PlayCard(card, playerID);
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
        return matches[UnityEngine.Random.Range(0, matches.Count)];
    }

    /// <summary>
    /// 条件に合うカードをすべて除外
    /// </summary>
    public void RemoveCards(Func<CardData, bool> condition)
    {
        handCardList.RemoveAll(c => condition(c));
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
}
