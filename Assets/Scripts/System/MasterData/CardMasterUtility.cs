using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity_CardData;

public class CardMasterUtility
{
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
}
