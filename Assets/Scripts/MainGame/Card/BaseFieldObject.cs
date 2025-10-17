using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFieldObject : MonoBehaviour
{
    public bool isLocal { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;

    public void SetIsLocal(bool setlocal)
    {
        isLocal = setlocal;
    }

    public void SetIsSelectable(bool setSelectable)
    {
        isSelectable = setSelectable;
    }
}
