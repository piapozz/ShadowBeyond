using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityManager;

public class ActiveAbility
{
    public TriggerTiming trigger { get; private set; }
    public BaseCondition condition { get; private set; }
    public BaseEffect effect { get; private set; }
}
