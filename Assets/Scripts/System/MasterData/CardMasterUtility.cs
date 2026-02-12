using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity_CardData;
using static GameEnum;

public class CardMasterUtility
{
    public static List<CardData> allCardList { get; private set; }

    public static void MakeCardDataList()
    {
        List<List<Param>> cardMasterList = MasterDataManager.cardData;
        allCardList = new List<CardData>();
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            for (int j = 0, paramMax = cardMasterList[i].Count; j < paramMax; j++)
            {
                if (cardMasterList[i][j].ID == -1) continue;
                CardData setCardData = GetCardData(cardMasterList[i][j]);
                setCardData.SetPackType((PackType)i);
                allCardList.Add(setCardData);
            }
        }
    }

    /// <summary>
    /// ID参照のカードのカードデータの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static CardData GetCardData(int ID)
    {
        List<List<Param>> cardMasterList = MasterDataManager.cardData;
        CardData cardData = null;
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            for (int j = 0, paramMax = cardMasterList[i].Count; j < paramMax; j++)
            {
                if (cardMasterList[i][j].ID != ID) continue;
                cardData = GetCardData(cardMasterList[i][j]);
                cardData.SetPackType((PackType)i);
            }
        }
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
            cardMaster.Defence,
            cardMaster.Token,
            cardMaster.Trait);
        // テキストデータ取得
        cardData.SetText(CardTextMasterUtility.GetCardText(cardMaster.ID));
        return cardData;
    }
}
