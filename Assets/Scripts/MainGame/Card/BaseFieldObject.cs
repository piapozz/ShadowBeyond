using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFieldObject : MonoBehaviour
{
    public bool isLocal { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;
    public event Action<BaseFieldObject> OnClick;

    public void SetIsLocal(bool setlocal)
    {
        isLocal = setlocal;
    }

    /// <summary>
    /// ‘I‘ğ‚Ì‰Â”Ûİ’è
    /// </summary>
    /// <param name="enable"></param>
    public void EnableSelectable(bool enable)
    {
        isSelectable = enable;
    }

    /// <summary>
    /// ‘I‘ğ‚³‚ê‚½‚Æ‚«
    /// </summary>
    public void SetSelected(bool select)
    {
        // Œ©‚½–Ú‚Ì•ÏX
    }

    public void OnPointerClick()
    {
        OnClick?.Invoke(this);
    }
}
