using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class KeywordAbilityInstance
{
    public KeywordAbility type;
    public ActiveAbility source; // null = Œ³‚©‚ç
    public int param;

    public KeywordAbilityInstance(KeywordAbility setKeyword, ActiveAbility setSource = null, int setParam = 0)
    {
        type = setKeyword;
        source = setSource;
        param = setParam;
    }
}
