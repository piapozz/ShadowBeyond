using System;
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
    private Button actButton = null;
    [SerializeField]
    private Button fusionButton = null;

    public void EnableUI(bool enable, string name = "", string detail = "", Action setActAction = null, Action setFusionAction = null)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            SetCardText(name, detail);
            SetButton(setActAction, setFusionAction);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        else
        {
            actButton.onClick.RemoveAllListeners();
            fusionButton.onClick.RemoveAllListeners();
        }
    }

    private void SetCardText(string name, string detail)
    {
        cardNameText.text = name;
        cardDetailText.text = detail;
    }

    private void SetButton(Action setActAction, Action setFusionAction)
    {
        if (setActAction == null)
        {
            actButton.gameObject.SetActive(false);
        }
        else
        {
            actButton.gameObject.SetActive(true);
            actButton.onClick.AddListener(() => setActAction());
        }
        if (setFusionAction == null)
        {
            fusionButton.gameObject.SetActive(false);
        }
        else
        {
            fusionButton.gameObject.SetActive(true);
            fusionButton.onClick.AddListener(() => setFusionAction());
        }
    }
}
