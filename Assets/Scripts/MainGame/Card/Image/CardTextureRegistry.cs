using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CardTextureRegistry : MonoBehaviour
{
    private static readonly Dictionary<int, Func<Texture>> _map
       = new Dictionary<int, Func<Texture>>();

    public static void AutoRegister()
    {
        var textures = Resources.LoadAll<Texture2D>("Image/Card")
            .Select(t =>
            {
                int id;
                if (!int.TryParse(t.name, out id))
                    return null;

                return new
                {
                    Texture = t,
                    Id = id
                };
            })
            .Where(x => x != null)
            .OrderBy(x => x.Id);

        foreach (var t in textures)
        {
            Register(t.Id, () => t.Texture);
        }
    }

    private static void Register(int id, Func<Texture> creator)
    {
        _map[id] = creator;
    }

    public static Texture Create(int id)
    {
        return _map[id]();
    }
    public static void Initialize()
    {
        AutoRegister();
    }

    public static Texture GetTexture(int id)
    {
        if (!_map.TryGetValue(id, out var ablity))
        {
            return null;
        }

        return ablity();
    }
}
