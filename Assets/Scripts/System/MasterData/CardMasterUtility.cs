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
                allCardList.Add(GetCardData(cardMasterList[i][j]));
            }
        }
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
        // テキストデータ取得
        cardData.SetText(CardTextMasterUtility.GetCardText(cardMaster.ID));
        return cardData;
    }

    /// <summary>
    /// ランダムなCardDataを取得
    /// </summary>
    /// <returns></returns>
    public static List<CardData> GetRandomCardData(int cardNum)
    {
        List<CardData> cardData = new List<CardData>();
        int cardCount = allCardList.Count;
        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = BattleManager.instance.rand.Next(0, cardCount);
            cardData.Add(allCardList[randomIndex]);
        }
        return cardData;
    }
}
