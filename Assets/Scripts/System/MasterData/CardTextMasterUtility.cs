using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        List<List<Param>> textMasterList = MasterDataManager.cardTextData;
        for (int i = 0, max = textMasterList.Count; i < max; i++)
        {
            for (int j = 0, paramMax = textMasterList[i].Count; j < paramMax; j++)
            {
                if (textMasterList[i][j].ID != ID) continue;

                return textMasterList[i][j];
            }
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
    /// ID参照のCardCrestTextの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static string GetCardCrestText(int ID)
    {
        var cardMaster = GetCardMaster(ID);
        if (cardMaster == null) return null;
        return cardMaster.CrestText;
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
