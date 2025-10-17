using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderObject : BaseFieldObject
{
    [SerializeField]
    private TextMeshPro defence = null;

    public Leader leader { get; private set; } = null;

    /// <summary>
    /// ƒŠ[ƒ_[‚Ìî•ñ‚ğİ’è
    /// </summary>
    /// <param name="setLeader"></param>
    public void SetLeader(Leader setLeader)
    {
        leader = setLeader;
        leader.SetGetObjectAction(() => { return this; });
    }

    public void SetDefenceText(int setDefence)
    {
        defence.text = setDefence.ToString();
    }
}
