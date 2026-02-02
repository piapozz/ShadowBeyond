using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddComboEffect : BaseEffect
{
    public AddComboEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(Leader targetLeader)
    {
        targetLeader.AddCombo(param[0]);
    }
}
