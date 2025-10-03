using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardLook : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro cardName = null;
    [SerializeField]
    private TextMeshPro cardCost = null;
    [SerializeField]
    private TextMeshPro cardAttack = null;
    [SerializeField]
    private TextMeshPro cardDefence = null;
    [SerializeField]
    private List<MeshRenderer> cardFrame = null;

    public void SetCardText(CardBase.CardData setCardData)
    {
        SetCardName(setCardData.m_name);
        SetCardCost(setCardData.m_cost);
        SetCardAttack(setCardData.m_status.m_attack);
        SetCardDefence(setCardData.m_status.m_health);
    }

    public void SetCardName(string setName)
    {
        if (cardName == null) return;
        cardName.text = setName;
    }

    public void SetCardCost(int setCost)
    {
        if (cardCost == null) return;
        cardCost.text = setCost.ToString();
    }

    public void SetCardAttack(int setAttack)
    {
        if (cardAttack == null) return;
        cardAttack.text = setAttack.ToString();
    }

    public void SetCardDefence(int setdefence)
    {
        if (cardDefence == null) return;
        cardDefence.text = setdefence.ToString();
    }

    public void SetCardMaterial(Material setMaterial)
    {
        for (int i = 0, max = cardFrame.Count; i < max; i++)
        {
            cardFrame[i].material = setMaterial;
        }
    }
}
