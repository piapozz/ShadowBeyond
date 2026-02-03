using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cardNameText = null;
    [SerializeField]
    private TextMeshProUGUI cardDetailText = null;
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private GameObject button = null;

    public void EnableUI(bool enable, string name = "", string detail = "")
    {
        gameObject.SetActive(enable);
        SetCardText(name, detail);
        SetButton();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private void SetCardText(string name, string detail)
    {
        cardNameText.text = name;
        cardDetailText.text = detail;
    }

    private void SetButton()
    {

    }
}
