using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCardFieldEffect : BaseEffect
{
    public EnterCardFieldEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(bool isOwn)
    {
        List<CardData> enterCardList = new List<CardData>();
        List<CardObject> enterCardObjectList = new List<CardObject>();

        for (int i = 0; i < param[1]; ++i) 
        { 
            // ƒJ[ƒh‚ð¶¬
            CardObject enterCard = UIManager.instance.GetNewCardObject(param[0]);
            enterCardObjectList.Add(enterCard);
            enterCardList.Add(enterCard.GetCardData());
        }
        BattleManager.instance.field.PlayCards(enterCardList, isOwn);
        UIManager.instance.EnterFieldSequence(enterCardObjectList, isOwn);

        return enterCardList;
    }
}
