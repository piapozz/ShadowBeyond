using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject infoPanel;

    [SerializeField]
    private Transform destroyInfo;

    [SerializeField]
    private Transform crestInfo;

    [SerializeField]
    private CardInfo cardInfoOrigin;

    [SerializeField]
    private Button activeButton;

    private List<CardInfo> destroyInfoList = new List<CardInfo>();
    private List<CardInfo> crestInfoList = new List<CardInfo>();

    // Start is called before the first frame update
    void Start()
    {
        RemoveAllInfo();
        infoPanel.SetActive(false);
        activeButton.onClick.AddListener(() => { infoPanel.SetActive(!infoPanel.activeSelf); });

    }

    public void AddDestroyInfo(CardData cardData)
    {
        var info = Instantiate(cardInfoOrigin, destroyInfo);
        info.SetInfo(cardData);
        destroyInfoList.Add(info);
    }

    public void AddCrestInfo(ActiveAbility crest)
    {
        var info = Instantiate(cardInfoOrigin, crestInfo);
        info.SetInfo(crest);
        crestInfoList.Add(info);
    }

    public void RemoveDestroyInfo(CardData cardData)
    {
        foreach (var info in destroyInfoList)
        {
            if (info.cardData == cardData)
            {
                destroyInfoList.Remove(info);
                Destroy(info.gameObject);
                break;
            }
        }
    }

    public void RemoveCrestInfo(ActiveAbility crest)
    {
        foreach (var info in crestInfoList)
        {
            if (info.crest == crest)
            {
                crestInfoList.Remove(info);
                Destroy(info.gameObject);
                break;
            }
        }
    }

    public void RemoveAllInfo()
    {
        foreach (var info in destroyInfoList)
        {
            Destroy(info.gameObject);
        }
        destroyInfoList.Clear();
        foreach (var info in crestInfoList)
        {
            Destroy(info.gameObject);
        }
        crestInfoList.Clear();
    }
}
