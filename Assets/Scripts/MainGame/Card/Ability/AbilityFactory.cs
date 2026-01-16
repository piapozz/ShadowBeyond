using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFactory
{
    private static readonly Dictionary<int, Func<BaseCardAbility>> abilityTable
         = new Dictionary<int, Func<BaseCardAbility>>();

    public static void Initialize()
    {
        Register(0, () => new CardAbility_Test());
    }

    private static void Register(int id, Func<BaseCardAbility> ability)
    {
        abilityTable[id] = ability;
    }

    public static BaseCardAbility GetAbility(int id)
    {
        if (!abilityTable.TryGetValue(id, out var ability))
        {
            return null;
        }

        return ability();
    }
}
