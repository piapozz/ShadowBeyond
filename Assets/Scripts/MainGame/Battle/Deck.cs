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
    private List<CardData> deckCardList = null;

    int playerID = 0;

    private Field field;
    private Hand hand;

    public void SetPlayerID(int index)
    {
        playerID = index;
        field = BattleManager.instance.field;
        hand = BattleManager.instance.GetPlayer(index).hand;
    }

    public void Init()
    {

    }

    public void SetDeckData(List<int> idList)
    {
        deckCardList = new List<CardData>();

        foreach (int id in idList)
        {
            deckCardList.Add(CardMasterUtility.GetCardData(id, true));
        }
    }

    // リストを取得
    public List<CardData> GetDeckCardList()
    {
        return deckCardList;
    }

    private bool CheckDeckOut()
    {
        if (deckCardList.Count == 0)
        {
            BattleManager.instance.NotifyDeckOutLose(playerID);
            return true;
        }
        return false;
    }


    /// <summary>
    /// デッキをシャッフルする
    /// </summary>
    public void ShuffleDeck()
    {
        int deckCount = deckCardList.Count;
        for (int i = deckCount - 1; i > 0; i--)
        {
            int n = BattleManager.instance.rand.Next(0, i + 1);
            CardData card = deckCardList[i];
            deckCardList[i] = deckCardList[n];
            deckCardList[n] = card;
        }

        UIManager.instance.ShuffleDeck(playerID);
    }

    /// <summary>
    /// デッキから指定枚数ドロー
    /// </summary>
    public List<CardData> DrawDeck(int drawCount)
    {
        List<CardData> drawCards = new List<CardData>(drawCount);

        for (int i = 0; i < drawCount; i++)
        {
            if (CheckDeckOut()) break;

            CardData card = deckCardList[0];
            deckCardList.RemoveAt(0);
            drawCards.Add(card);
            hand.AddCard(card);
        }

        if (drawCards.Count > 0)
            UIManager.instance.DrawCards(playerID, drawCards);

        return drawCards;
    }

    /// <summary>
    /// デッキから指定枚数引いて手札に加えない
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<CardData> PeekDeck(int count)
    {
        List<CardData> peekedCards = new List<CardData>();

        for (int i = 0; i < count; i++)
        {
            if (CheckDeckOut()) break;

            CardData card = deckCardList[0];
            deckCardList.RemoveAt(0);
            peekedCards.Add(card);

            CardObject cardObject = UIManager.instance.GetUnuseCardObject();
        }

        return peekedCards;
    }


    /// <summary>
    /// 指定カードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToHand(CardData card)
    {
        if (card == null) return;
        hand.AddCard(card);
        deckCardList.Remove(card);
        List<CardData> addCards = new List<CardData> { card };
        UIManager.instance.DrawCards(playerID, addCards);
    }

    /// <summary>
    /// 指定カードを場に出す
    /// </summary>
    /// <param name="card"></param>
    public void PlayDeckToField(CardData card)
    {
        if (card == null) return;
        field.EnterCard(card, playerID == 0 ? true : false);
        deckCardList.Remove(card);
        UIManager.instance.PlayDeckToField(playerID, card);
    }

    /// <summary>
    /// 指定カードを引く
    /// </summary>
    /// <param name="drawCards"></param>
    public List<CardData> DrawDeck(List<CardData> drawCards, int drawCount = -1)
    {
        List<CardData> cardList = new List<CardData>();

        if (drawCount == -1)
            drawCount = drawCards.Count;

        for (int i = 0; i < drawCount; i++)
        {
            if (CheckDeckOut()) break;

            CardData card = drawCards[i];
            if (!deckCardList.Contains(card))
                continue;

            deckCardList.Remove(card);
            cardList.Add(card);
            hand.AddCard(card);
        }

        if (cardList.Count > 0)
            UIManager.instance.DrawCards(playerID, cardList);

        return cardList;
    }


    /// <summary>
    /// 指定カードを引く
    /// </summary>
    /// <param name="condition"></param>
    public List<CardData> DrawDeck(Func<CardData, bool> condition, int drawCount = -1)
    {
        var targets = GetCards(condition);
        if (targets.Count == 0 && CheckDeckOut())
            return new List<CardData>();
        if (targets.Count < drawCount)
            drawCount = targets.Count;

        return DrawDeck(targets, drawCount);
    }


    /// <summary>
    /// 条件に合うカードをすべて取得
    /// </summary>
    public List<CardData> GetCards(Func<CardData, bool> condition)
    {
        return deckCardList.Where(condition).ToList();
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
        deckCardList.RemoveAll(c => condition(c));

        UIManager.instance.RemoveDeckCards(playerID);
    }

    /// <summary>
    /// デッキにカードを追加
    /// </summary>
    public void AddCard(CardData card)
    {
        deckCardList.Add(card);
    }

    /// <summary>
    /// デッキ枚数を取得
    /// </summary>
    public int GetDeckCount()
    {
        return deckCardList.Count;
    }

    /// <summary>
    /// 名前の種類数を取得
    /// </summary>
    public int GetUniqueNameCount()
    {
        return deckCardList.Select(c => c.name).Distinct().Count();
    }

    /// <summary>
    /// デッキの最小コストのカードを取得
    /// </summary>
    public CardData GetMinCostCard()
    {
        return deckCardList.OrderBy(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// デッキの最大コストのカードを取得
    /// </summary>
    public CardData GetMaxCostCard()
    {
        return deckCardList.OrderByDescending(c => c.cost).FirstOrDefault();
    }

    /// <summary>
    /// デッキ内に重複するカードがあるか
    /// </summary>
    public bool HasDuplicate()
    {
        return deckCardList.GroupBy(c => c.id).Any(g => g.Count() > 1);
    }

    /// <summary>
    /// デッキを入れ替える
    /// </summary>
    public void ReplaceDeck(List<CardData> newDeck)
    {
        deckCardList = new List<CardData>(newDeck);
    }
}
