using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderUI : MonoBehaviour
{
    [SerializeField]
    private LeaderObject ownLeaderObject = null;
    [SerializeField]
    private LeaderObject opponentLeaderObject = null;

    public void Initialize(Leader setLeader, int index)
    {
        if (index == 0)
        {
            ownLeaderObject.SetIsLocal(true);
            ownLeaderObject.Initialize(setLeader);
        }
        else if(index == 1)
        {
            opponentLeaderObject.SetIsLocal(false);
            opponentLeaderObject.Initialize(setLeader);
        }
    }
}
