using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardHandEffect : BaseEffect
{
    public AddCardHandEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(Hand targetHand)
    {
        for (int i = 0, max = param[1]; i < max; i++)
        {
            CardData addCard = CardMasterUtility.GetCardData(param[0]);
            targetHand.AddCard(addCard);
        }
    }
}
