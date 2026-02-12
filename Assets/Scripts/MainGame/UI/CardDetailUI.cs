using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEnum;

public class CardDetailUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cardNameText = null;
    [SerializeField]
    private TextMeshProUGUI cardCostText = null;
    [SerializeField]
    private TextMeshProUGUI cardDetailText = null;
    [SerializeField]
    private TextMeshProUGUI cardTypeText = null;
    [SerializeField]
    private GameObject actArea = null;
    [SerializeField]
    private GameObject fusionArea = null;
    [SerializeField]
    private Button actButton = null;
    [SerializeField]
    private Button fusionButton = null;

    public void EnableUI(bool enable, CardData card, Action setActAction = null, Action setFusionAction = null, bool isOwnTurn = false)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            SetCardText(card.name, card.defaultCost, card.text, card.typeDetail);
            SetButton(setActAction, setFusionAction, isOwnTurn, card.canAct, card.canFusion);
        }
        else
        {
            actButton.onClick.RemoveAllListeners();
            fusionButton.onClick.RemoveAllListeners();
        }
    }

    public void EnableUI(bool enable, string name = "", string detail = "", int cost = -1, List<CardTypeDetail> type = null)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            SetCardText(name, cost, detail, type);
        }
    }

    private void SetCardText(string name,int cost, string detail, List<CardTypeDetail> type)
    {
        cardNameText.text = name;
        cardCostText.text = cost.ToString();
        cardDetailText.text = detail;
        string typeText = "";
        if (type != null)
        {
            for (int i = 0, max = type.Count; i < max; i++)
            {
                typeText += ToText(type[i]);
            }
        }
        cardTypeText.text = typeText;
    }

    private void SetButton(Action setActAction, Action setFusionAction, bool isOwn, bool canAct, bool canFusion)
    {
        if (setActAction == null)
        {
            actArea.gameObject.SetActive(false);
        }
        else
        {
            actArea.gameObject.SetActive(true);
            actButton.onClick.AddListener(() => setActAction());
            actButton.interactable = isOwn && canAct;
        }
        if (setFusionAction == null)
        {
            fusionArea.gameObject.SetActive(false);
        }
        else
        {
            fusionArea.gameObject.SetActive(true);
            fusionButton.onClick.AddListener(() => setFusionAction());
            fusionButton.interactable = isOwn && canFusion;
        }
    }
}
