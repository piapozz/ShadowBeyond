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
            ownLeaderObject.Initialize(setLeader);
            ownLeaderObject.SetIsLocal(true);
        }
        else if(index == 1)
        {
            opponentLeaderObject.Initialize(setLeader);
            opponentLeaderObject.SetIsLocal(false);
        }
    }
}
