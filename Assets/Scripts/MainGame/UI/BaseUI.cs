using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private Transform root;

    public void SetCanvas(Canvas canvas)
    {
        if (root == null) root = this.transform;
        root.SetParent(canvas.transform, false);
    }

    public void Open()
    {
        root.gameObject.SetActive(true);
    }

    public void Close()
    {
        root.gameObject.SetActive(false);
    }
}
