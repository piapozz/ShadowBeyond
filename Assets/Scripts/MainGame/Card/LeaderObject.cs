using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderObject : BaseFieldObject
{
    [SerializeField]
    private TextMeshPro defence = null;

    public Leader leader { get; private set; } = null;

    public void SetLeader(Leader setLeader)
    {
        leader = setLeader;
    }

    public void SetDefenceText(int setDefence)
    {
        defence.text = setDefence.ToString();
    }
}
