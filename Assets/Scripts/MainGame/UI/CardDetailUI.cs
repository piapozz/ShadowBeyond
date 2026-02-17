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

    public void EnableUI(bool enable, bool isInGame, CardData card = null, Action setActAction = null, Action setFusionAction = null, bool isOwnTurn = false)
    {
        gameObject.SetActive(enable);
        if (enable)
        {
            SetCardText(card.name, card.defaultCost, card.text, card.crestText, card.typeDetail);
            if (isInGame)
                SetButton(setActAction, setFusionAction, isOwnTurn, card.canAct, card.canFusion);
            else
                UnactiveButton();
        }
        else
        {
            if (!isInGame) return;
            actButton.onClick.RemoveAllListeners();
            fusionButton.onClick.RemoveAllListeners();
        }
    }

    private void SetCardText(string name,int cost, string detail, string crest, List<CardTypeDetail> type)
    {
        cardNameText.text = name;
        cardCostText.text = cost.ToString();
        cardDetailText.text = detail;
        if (crest != "")
        {
            cardDetailText.text += "\n" + crest;
        }

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
            actButton.onClick.RemoveAllListeners();
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
            fusionButton.onClick.RemoveAllListeners();
            fusionButton.onClick.AddListener(() => setFusionAction());
            fusionButton.interactable = isOwn && canFusion;
        }
    }

    private void UnactiveButton()
    {
        actArea.gameObject.SetActive(false);
        fusionArea.gameObject.SetActive(false);
    }
}
