using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class KeywordAbilityInstance
{
    public KeywordAbility type { get; private set; }
    public CardData source { get; private set; } // null = Œ³‚©‚ç
    public int param { get; private set; }

    public KeywordAbilityInstance(KeywordAbility setKeyword, CardData setSource = null, int setParam = 0)
    {
        type = setKeyword;
        source = setSource;
        param = setParam;
    }

    public void AddParam(int addParam)
    {
        param += addParam;
    }

    public void RemoveParam(int removeParam)
    {
        param -= removeParam;
        if (param < 0)
            param = 0;
    }

    public bool IsNoCount()
    {
        return param == 0;
    }
}
