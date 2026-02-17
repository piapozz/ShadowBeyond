using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI costText;

    [SerializeField]
    private Button detailButton;

    public CardData cardData { get; private set; }
    public ActiveAbility crest { get; private set; }

    public void SetInfo(CardData setCardData)
    {
        nameText.text = setCardData.name;
        costText.text = setCardData.cost.ToString();
        cardData = setCardData;
        detailButton.onClick.AddListener(() => { UIManager.instance.SetCardDetailUI(cardData.GetCardObject()); });
    }

    public void SetInfo(ActiveAbility setCrest)
    {
        nameText.text = setCrest.sourceCard.name;
        costText.text = setCrest.sourceCard.cost.ToString();
        detailButton.onClick.AddListener(() => { UIManager.instance.SetCardDetailUI(setCrest.sourceCard.GetCardObject()); });
    }

    public void ClearInfo()
    {
        nameText.text = "";
        costText.text = "";
        cardData = null;
        crest = null;
        detailButton.onClick.RemoveAllListeners();
    }
}
