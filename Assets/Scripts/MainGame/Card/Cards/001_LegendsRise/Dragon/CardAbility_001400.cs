using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【ファンファーレ】『ベビーファイアドレイク』1枚を自分の場に出す。
// 【進化時】【ファンファーレ】と同じ能力が働く。
public class CardAbility_001400 : BaseCardAbility
{
    private const int FIRE_DRAKE_WHELP_ID = 001419;

    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        EnterCardFieldEffect enterCardFieldEffect = new EnterCardFieldEffect(new List<int> { FIRE_DRAKE_WHELP_ID, 1 });
        enterCardFieldEffect.ExecuteEffect(isOwn);
    }

    public override void Evolve(bool isOwn, List<BaseComponent> selected = null)
    {
        Fanfare(isOwn, selected);
    }
}
