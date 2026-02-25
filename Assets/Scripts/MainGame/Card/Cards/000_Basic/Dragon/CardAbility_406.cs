using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_406 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        // 自分のPP最大値を + 1する。その後、自分のPP最大値が10なら、自分のデッキから1枚を引く。
        var player = GetPlayer(isOwn);
        GainMaxPlayPointEffect gainMaxPlayPointEffect = new GainMaxPlayPointEffect(new List<int> { 1 });
        gainMaxPlayPointEffect.ExecuteEffect(player.leader);
        if (player.leader.maxPlayPoint < 10) return;
        DrawEffect drawEffect = new DrawEffect(new List<int> { 1 } );
        drawEffect.ExecuteEffect(player.deck);
    }
}
