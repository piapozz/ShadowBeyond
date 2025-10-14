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
        List<Param> cardMasterList = MasterDataManager.cardData[0];
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            if (cardMasterList[i].ID != ID) continue;

            return cardMasterList[i];
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
    public static CardData GetRandomCardData()
    {
        List<Param> cardMasterList = MasterDataManager.cardData[0];
        int randomIndex = BattleManager.instance.rand.Next(0, cardMasterList.Count);
        return GetCardData(cardMasterList[randomIndex]);
    }
}
