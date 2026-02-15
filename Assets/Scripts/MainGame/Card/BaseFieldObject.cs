using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFieldObject : MonoBehaviour
{
    [SerializeField]
    protected List<Material> outLineMaterials = null;

    public bool isLocal { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;
    public event Action<BaseFieldObject> OnClick;

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

        // 見た目変更
        SetObjectOutLine(OutLineType.Selectable);
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
        // 見た目の変更
    }

    public void OnPointerClick()
    {
        OnClick?.Invoke(this);
    }
}
