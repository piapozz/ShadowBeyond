using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【ファンファーレ】【覚醒】なら、これは進化する。
// これが進化したとき、自分のPP最大値を+1する。

public class CardAbility_001411 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
    }

    public override void Fanfare(bool isOwn, List<BaseComponent> selected = null)
    {
        if (!GetPlayer(isOwn).leader.IsOverflow()) return;
        EvolveEffect evolveEffect = new EvolveEffect(null);
        evolveEffect.ExecuteEffect(sourceData);
    }

    public override void AutoEvolve(bool isOwn)
    {
        var player = GetPlayer(isOwn);
        GainMaxPlayPointEffect gainMaxPlayPointEffect = new GainMaxPlayPointEffect(new List<int> { 1 });
        gainMaxPlayPointEffect.ExecuteEffect(player.leader);
    }

    public override void AutoSuperEvolve(bool isOwn)
    {
        var player = GetPlayer(isOwn);
        GainMaxPlayPointEffect gainMaxPlayPointEffect = new GainMaxPlayPointEffect(new List<int> { 1 });
        gainMaxPlayPointEffect.ExecuteEffect(player.leader);
    }
}
