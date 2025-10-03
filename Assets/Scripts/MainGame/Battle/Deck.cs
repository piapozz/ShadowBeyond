using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

// デッキに加える
// デッキから引く
// デッキから場に出す
// デッキからフォロワーを加える
// デッキからスペルを加える
// デッキからアミュレットを加える
// デッキから～タイプを加える
// デッキから指定カードを手札に加える
// デッキからカードを消滅させる
// デッキの指定カードをすべて取得
// デッキの指定カードをすべて除外
// デッキの枚数を取得
// デッキの～をフォロワーを取得
// デッキの～の能力を持つカードを加える
// デッキの～コスト以下のカードを加える
// デッキの～コスト以下のカードを場に出す
// デッキの～コスト以上のカードを加える
// デッキの～コスト以上のカードを場に出す
// デッキの～コストのカードを加える
// デッキの～コストのカードを場に出す
// デッキのコスト最少のカードを加える
// デッキのコスト最大のカードを加える
// デッキに重複するカードがあるか
// デッキのカードに～タイプを付与
// デッキの体力～のカードを加える
// デッキの攻撃力～のカードを加える
// デッキのカードの攻撃力を～する
// デッキのカードの体力を～する
// デッキのカードのコストを～する
// デッキのカードの名前の種類を取得
// デッキから名前の異なるカードをデッキに加える
// デッキを指定カードリストに入れ替える

/// <summary>
/// デッキクラス
/// </summary>
public class Deck
{
    public List<CardData> _deckCardList;

    public void Init(List<CardData> initialCards)
    {
        _deckCardList = new List<CardData>(initialCards);
        ShuffleDeck();
    }

    /// <summary>
    /// デッキをシャッフルする
    /// </summary>
    public void ShuffleDeck()
    {
        int deckCount = _deckCardList.Count;
        for (int i = deckCount - 1; i > 0; i--)
        {
            int n = UnityEngine.Random.Range(0, i + 1);
            CardData card = _deckCardList[i];
            _deckCardList[i] = _deckCardList[n];
            _deckCardList[n] = card;
        }
    }

    /// <summary>
    /// デッキから指定枚数ドロー
    /// </summary>
    public async UniTask<List<CardData>> DrawDeck(int drawCount)
    {
        List<CardData> drawCards = new List<CardData>();

        for (int i = 0; i < drawCount; i++)
        {
            if (_deckCardList.Count == 0) break;
            CardData card = _deckCardList[0];
            _deckCardList.RemoveAt(0);
            drawCards.Add(card);
        }

        return drawCards;
    }

    /// <summary>
    /// 指定カードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToHand(CardData card)
    {
        if (card == null) return;
        // hand.AddCard(card);
        _deckCardList.Remove(card);
    }

    /// <summary>
    /// 指定カードを場に出す
    /// </summary>
    /// <param name="card"></param>
    public void PlayCardToField(CardData card)
    {
        if (card == null) return;
        // field.PlayCard(card);
        _deckCardList.Remove(card);
    }

    /// <summary>
    /// 条件に合うカードをすべて取得
    /// </summary>
    public List<CardData> GetCards(Func<CardData, bool> condition)
    {
        return _deckCardList.Where(condition).ToList();
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
        _deckCardList.RemoveAll(c => condition(c));
    }

    /// <summary>
    /// デッキにカードを追加
    /// </summary>
    public void AddCard(CardData card)
    {
        _deckCardList.Add(card);
    }

    /// <summary>
    /// デッキ枚数を取得
    /// </summary>
    public int GetDeckCount()
    {
        return _deckCardList.Count;
    }

    /// <summary>
    /// 名前の種類数を取得
    /// </summary>
    public int GetUniqueNameCount()
    {
        return _deckCardList.Select(c => c.name).Distinct().Count();
    }

    /// <summary>
    /// デッキの最小コストのカードを取得
    /// </summary>
    public CardData GetMinCostCard()
    {
        return _deckCardList.OrderBy(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// デッキの最大コストのカードを取得
    /// </summary>
    public CardData GetMaxCostCard()
    {
        return _deckCardList.OrderByDescending(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// デッキ内に重複するカードがあるか
    /// </summary>
    public bool HasDuplicate()
    {
        return _deckCardList.GroupBy(c => c.id).Any(g => g.Count() > 1);
    }

    /// <summary>
    /// デッキを入れ替える
    /// </summary>
    public void ReplaceDeck(List<CardData> newDeck)
    {
        _deckCardList = new List<CardData>(newDeck);
    }
}
