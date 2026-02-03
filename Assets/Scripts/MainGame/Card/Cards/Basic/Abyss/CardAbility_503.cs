using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_503 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn)
    {
        // 【モード】1つを選んでその能力が働く。
        //（1）自分のデッキからフォロワー1枚を引く。
        //（2）【リアニメイト_2】を行う。

        int index = 0;
        index = BattleManager.instance.rand.Next(0, 2);

        switch (index)
        {
            case 0:
                var targetPlayer = GetPlayer(isOwn);
                DrawEffect drawEffect = new DrawEffect(new List<int> { 1 });
                drawEffect.ExecuteEffect(targetPlayer.deck, (card) => { return card.type == GameEnum.CardType.FOLLOWER; });
                break;
            case 1:

                break;
        }

    }
}
