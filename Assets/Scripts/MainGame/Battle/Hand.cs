using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 手札クラス
public class Hand 
{
    public List<CardData> _handCardList;

    public const int MAX_HAND = 9;

    public void Init(List<CardData> initialCards)
    {
        _handCardList = new List<CardData>(initialCards);
    }

    /// <summary>
    /// 指定カードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(CardData card)
    {
        if (card == null) return;
        if (_handCardList.Count >= MAX_HAND) return;
        _handCardList.Add(card);
    }

    /// <summary>
    /// 指定カードを場に出す
    /// </summary>
    /// <param name="card"></param>
    public void PlayCardToField(CardData card)
    {
        if (card == null) return;
        // field.PlayCard(card);
        _handCardList.Remove(card);
    }

    /// <summary>
    /// 条件に合うカードをすべて取得
    /// </summary>
    public List<CardData> GetCards(Func<CardData, bool> condition)
    {
        return _handCardList.Where(condition).ToList();
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
        _handCardList.RemoveAll(c => condition(c));
    }

    /// <summary>
    /// 手札枚数を取得
    /// </summary>
    public int GetHandCount()
    {
        return _handCardList.Count;
    }

    /// <summary>
    /// 手札の最小コストのカードを取得
    /// </summary>
    public CardData GetMinCostCard()
    {
        return _handCardList.OrderBy(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// 手札の最大コストのカードを取得
    /// </summary>
    public CardData GetMaxCostCard()
    {
        return _handCardList.OrderByDescending(c => c.cost).FirstOrDefault();
    }
}
