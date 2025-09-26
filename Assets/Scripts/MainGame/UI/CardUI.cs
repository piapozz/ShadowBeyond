using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultCard;
    [SerializeField] private GameObject handCard;
    [SerializeField] private GameObject fieldCard;

    public enum CardState
    {
        Default,
        Hand,
        Field
    }

    private CardState currentState = CardState.Default;

    public void SetCardState(CardState state)
    {
        currentState = state;
        switch (currentState)
        {
            case CardState.Default:
                defaultCard.SetActive(true);
                handCard.SetActive(false);
                fieldCard.SetActive(false);
                break;
            case CardState.Hand:
                defaultCard.SetActive(false);
                handCard.SetActive(true);
                fieldCard.SetActive(false);
                break;
            case CardState.Field:
                defaultCard.SetActive(false);
                handCard.SetActive(false);
                fieldCard.SetActive(true);
                break;
        }
    }
}
