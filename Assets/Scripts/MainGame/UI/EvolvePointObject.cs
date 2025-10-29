using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolvePointObject : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer = null;

    private bool isSuperEvolve = false;
    private bool isLocal = false;

    public void Initialize(bool setIsSuperEvolve, bool setIsLocal)
    {
        isSuperEvolve = setIsSuperEvolve;
        isLocal = setIsLocal;
    }

    private void OnMouseDown()
    {
        // 自分のターンか自分の進化権か判定
        if (!BattleManager.instance.IsOwnTurn() || !isLocal) return;
        // 進化権があるか判定
        // 線を出す
        UIManager.instance.SetLineRenderer(lineRenderer, transform);
        lineRenderer.enabled = true;
    }

    private void OnMouseDrag()
    {
        // 攻撃の線を出す
        UIManager.instance.SetLineRenderer(lineRenderer, transform);
    }

    private void OnMouseUp()
    {
        lineRenderer.enabled = false;

        // 進化処理
        // 自分のフィールドの未進化のフォロワーなら進化可能
    }
}
