using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingMessage : MonoBehaviour
{
    public static MatchingMessage Instance { get; private set; }

    [SerializeField]
    private GameObject matchingMessagePanel;

    [SerializeField]
    private TextMeshProUGUI text;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        matchingMessagePanel.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void ShowMessage(string message)
    {
        text.text = message;
        matchingMessagePanel.SetActive(true);
    } 

    public void HideMessage()
    {
        matchingMessagePanel.SetActive(false);
    }
}
