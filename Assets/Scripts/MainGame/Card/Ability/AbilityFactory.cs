using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class AbilityFactory
{
    private static readonly Dictionary<int, Func<BaseCardAbility>> _map
        = new Dictionary<int, Func<BaseCardAbility>>();

    public static void AutoRegister()
    {
        var abilityTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.IsSubclassOf(typeof(BaseCardAbility)) &&
                t.Name.StartsWith("CardAbility_"))
            .Select(t => new
            {
                Type = t,
                Id = int.Parse(t.Name.Replace("CardAbility_", ""))
            })
            .OrderBy(x => x.Id);

        foreach (var a in abilityTypes)
        {
            Register(a.Id, () => (BaseCardAbility)Activator.CreateInstance(a.Type));
        }
    }

    private static void Register(int id, Func<BaseCardAbility> creator)
    {
        _map[id] = creator;
    }

    public static BaseCardAbility Create(int id)
    {
        return _map[id]();
    }
    public static void Initialize()
    {
        AutoRegister();
    }

    public static BaseCardAbility GetAbility(int id)
    {
        if(!_map.TryGetValue(id, out var ablity))
        {
            return null;
        }

        return ablity();
    }
}
