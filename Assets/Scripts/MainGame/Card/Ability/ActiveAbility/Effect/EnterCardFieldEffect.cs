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

        for (int i = 0; i < param[1]; ++i) 
        { 
            // カードを生成
            CardObject enterCard = UIManager.instance.GetNewCardObject(param[0]);
            if (isOwn)
            {
                // 自分の場に出す
            }
            else
            {
                // 相手の場に出す
                
            }

            enterCardList.Add(enterCard.cardData);
        }

        return enterCardList;
    }
}
