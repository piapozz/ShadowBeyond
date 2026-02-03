using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMaxPlayPointEffect : BaseEffect
{
    public GainMaxPlayPointEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(Leader targetLeader)
    {
        targetLeader.SetMaxEvolvePoint(targetLeader.maxPlayPoint + param[0]);
    }
}
