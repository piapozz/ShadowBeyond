using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;
using static Entity_CardTextData;



public class CardTextMasterUtility : MonoBehaviour
{
    /// <summary>
    /// ID参照のカードのマスターデータの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static Param GetCardMaster(int ID)
    {
        List<Param> cardMasterList = MasterDataManager.cardTextData[0];
        for (int i = 0, max = cardMasterList.Count; i < max; i++)
        {
            if (cardMasterList[i].ID != ID) continue;

            return cardMasterList[i];
        }
        return null;
    }

    /// <summary>
    /// ID参照のCardTextの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static string GetCardText(int ID)
    {
        var cardMaster = GetCardMaster(ID);
        if (cardMaster == null) return null;
        return cardMaster.CardText;
    }

    /// <summary>
    /// カードのマスターデータからCardTextを取得
    /// </summary>
    /// <param name="cardMaster"></param>
    /// <returns></returns>
    public static string GetCardText(Param cardMaster)
    {
        return cardMaster.CardText;
    }
}
