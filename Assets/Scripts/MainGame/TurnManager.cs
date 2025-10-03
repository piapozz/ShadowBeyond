using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public int turn { get; private set; } = -1;
    public int turnPlayer { get; private set; } = -1;

    public TurnManager()
    {
        turn = 0;
        turnPlayer = 0;
    }


}
