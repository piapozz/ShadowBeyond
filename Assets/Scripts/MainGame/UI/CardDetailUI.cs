using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDetailUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cardNameText = null;
    [SerializeField]
    private TextMeshProUGUI cardDetailText = null;

    public void EnableUI(bool enable, string name = "", string detail = "")
    {
        gameObject.SetActive(enable);
        SetCardText(name, detail);
    }

    private void SetCardText(string name, string detail)
    {
        cardNameText.text = name;
        cardDetailText.text = detail;
    }
}
