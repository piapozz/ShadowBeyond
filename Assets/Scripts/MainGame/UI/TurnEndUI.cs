using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TurnEndUI : MonoBehaviour
{
    [SerializeField]
    private GameObject ownTurnButton = null;
    [SerializeField]
    private GameObject opponentTurnObject = null;
    [SerializeField]
    private Button turnEndButton = null;

    public void SetButtonEnable(bool isOwnTurn)
    {
        if (isOwnTurn)
        {
            ownTurnButton.SetActive(true);
            opponentTurnObject.SetActive(false);
        }
        else
        {
            ownTurnButton.SetActive(false);
            opponentTurnObject.SetActive(true);
        }
    }

    public void SetButtonAction(Action setAction)
    {
        turnEndButton.onClick.AddListener(() => setAction());
    }
}
