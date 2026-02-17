using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_101 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // Ž©•ª‚ÌƒRƒ“ƒ{‚ð+1‚·‚é
        AddComboEffect effect = new AddComboEffect(new List<int> { 1 });
        var targetPlayer = GetPlayer(isOwn);
        effect.ExecuteEffect(targetPlayer.leader);
    }
}
