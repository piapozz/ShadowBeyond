using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddComboEffect : BaseEffect
{
    public AddComboEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        Leader targetLeader = context.player.leader;
        if (targetLeader == null) return null;

        ExecuteEffect(targetLeader);

        return null;
    }

    public override void ExecuteEffect(Leader targetLeader)
    {
        targetLeader.AddCombo(param[0]);
    }
}
