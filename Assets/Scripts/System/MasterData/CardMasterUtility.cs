using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity_CardData;
using static GameEnum;

public class CardMasterUtility
{
    /// <summary>
    /// ID参照のカードのマスターデータの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static Param GetCardMaster(int ID)
    {
        List<List<Param>> cardMasterList = MasterDataManager.cardData;
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            for (int j = 0, paramMax = cardMasterList[i].Count; j < paramMax; j++)
            {
                if (cardMasterList[i][j].ID != ID) continue;

                return cardMasterList[i][j];
            }
        }
        return null;
    }

    /// <summary>
    /// ID参照のCardDataの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static CardData GetCardData(int ID)
    {
        var cardMaster = GetCardMaster(ID);
        if (cardMaster == null) return null;
        CardData cardData = new CardData(
            cardMaster.ID,
            (LeaderClass)cardMaster.Class,
            (CardRarity)cardMaster.Rarity,
            (CardType)cardMaster.Type,
            cardMaster.Name,
            cardMaster.Cost,
            cardMaster.Attack,
            cardMaster.Defence);
        return cardData;
    }

    /// <summary>
    /// カードのマスターデータからCardDataを取得
    /// </summary>
    /// <param name="cardMaster"></param>
    /// <returns></returns>
    public static CardData GetCardData(Param cardMaster)
    {
        CardData cardData = new CardData(
            cardMaster.ID,
            (LeaderClass)cardMaster.Class,
            (CardRarity)cardMaster.Rarity,
            (CardType)cardMaster.Type,
            cardMaster.Name,
            cardMaster.Cost,
            cardMaster.Attack,
            cardMaster.Defence);
        return cardData;
    }

    /// <summary>
    /// ランダムなCardDataを取得
    /// </summary>
    /// <returns></returns>
    public static List<CardData> GetRandomCardData(int cardNum)
    {
        List<CardData> cardMasterData = GetAllCardData();
        List<CardData> cardData = new List<CardData>();
        int cardCount = cardMasterData.Count;
        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = BattleManager.instance.rand.Next(0, cardCount);
            cardData.Add(cardMasterData[randomIndex]);
        }
        return cardData;
    }

    public static List<CardData> GetAllCardData()
    {
        List<List<Param>> cardMasterList = MasterDataManager.cardData;
        List<CardData> cardList = new List<CardData>();
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            for (int j = 0, paramMax = cardMasterList[i].Count; j < paramMax; j++)
            {
                if (cardMasterList[i][j].ID == -1) continue;
                cardList.Add(GetCardData(cardMasterList[i][j]));
            }
        }
        return cardList;
    }
}
