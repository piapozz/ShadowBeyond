using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardData;

public class CardLook : MonoBehaviour
{
    [SerializeField]
    private GameObject cardObject = null;
    [SerializeField]
    private TextMeshPro cardName = null;
    [SerializeField]
    private TextMeshPro cardCost = null;
    [SerializeField]
    private TextMeshPro cardAttack = null;
    [SerializeField]
    private TextMeshPro cardDefence = null;
    [SerializeField]
    private MeshRenderer cardFrame = null;
    [SerializeField]
    private GameObject imageObj = null;

    public void SetCardText(CardData setCardData)
    {
        SetCardName(setCardData.name);
        SetCardCost(setCardData.cost);
        FollowerStatus status = setCardData.GetCurrentStatus();
        SetCardAttack(status.m_attack);
        SetCardDefence(status.m_defance);
        SetCardImae(setCardData.id);
    }

    public void SetCardImae(int id)
    {
         Texture tex = CardTextureRegistry.GetTexture(id);
        if (tex == null) return;
        Material imageMaterial = imageObj.GetComponent<MeshRenderer>().material;
        imageMaterial.SetTexture("_MainTex", tex);
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
        cardFrame.material = setMaterial;
    }

    public void SetCardFrontActive(bool active)
    {
        if (cardObject != null) cardObject.SetActive(active);
        if (cardName != null) cardName.enabled = active;
        if (cardCost != null) cardCost.enabled = active;
        if (cardAttack != null) cardAttack.enabled = active;
        if (cardDefence != null) cardDefence.enabled = active;
    }
}
