using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardHandEffect : BaseEffect
{
    public AddCardHandEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(Hand targetHand)
    {
        List<CardData> addCardList = new List<CardData>();
        List<CardObject> addCardObjectList = new List<CardObject>();

        for (int i = 0; i < param[1]; ++i)
        {
            // ƒJ[ƒh‚ð¶¬
            CardObject enterCard = UIManager.instance.GetNewCardObject(param[0]);
            enterCard.SetCardState(CardObject.CardState.HAND);
            addCardObjectList.Add(enterCard);
            addCardList.Add(enterCard.cardData);
        }
        targetHand.AddCards(addCardList);
        UIManager.instance.AddHandCard(targetHand.playerID, addCardList);

        return addCardList;
    }
}
