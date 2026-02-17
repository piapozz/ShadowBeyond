using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseFieldObject : MonoBehaviour
{
    [SerializeField]
    protected List<Material> outLineMaterials = null;

    public bool isLocal { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;
    public event Action<BaseFieldObject> OnClick;
    public BaseComponent component { get; protected set; } = null;

    public enum OutLineType
    {
        None = 0,
        CanAttackFollower,
        CanAttackLeader,
        Selectable,
        IsSelect
    }

    public void SetIsLocal(bool setlocal)
    {
        isLocal = setlocal;
    }

    /// <summary>
    /// 選択の可否設定
    /// </summary>
    /// <param name="enable"></param>
    public void EnableSelectable(bool enable)
    {
        isSelectable = enable;

        OutLineType outLine = enable ? OutLineType.Selectable : OutLineType.None;
        // 見た目変更
        SetObjectOutLine(outLine);
    }

    /// <summary>
    /// オブジェクト外周の見た目の変更
    /// </summary>
    /// <param name="type"></param>
    public abstract void SetObjectOutLine(OutLineType type);

    /// <summary>
    /// 選択されたとき
    /// </summary>
    public void SetSelected(bool select)
    {
        OutLineType outLine = select ? OutLineType.IsSelect : OutLineType.Selectable;
        // 見た目変更
        SetObjectOutLine(outLine);
    }

    public void OnPointerClick()
    {
        OnClick?.Invoke(this);
    }
}
