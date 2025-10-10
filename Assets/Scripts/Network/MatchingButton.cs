using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchingButton : MonoBehaviour
{

    [SerializeField]
    private Button matchingButton;

    [SerializeField]
    private TextMeshProUGUI matchingButtonText;

    // Start is called before the first frame update
    void Start()
    {
        // ボタンイベント設定
        matchingButton.onClick.AddListener(() =>
        {
            Debug.Log("MatchingButton clicked");
            NetworkManager.Instance.StartMatchmaking();
        });
        // テキストの設定
        matchingButtonText.text = "マッチング開始";
    }

    // Update is called once per frame
    void Update()
    {
        // ボタンを押している間はテキストを変更する
        if (NetworkManager.Instance.IsConnected()) 
        {
            matchingButtonText.text = "マッチング中...";
            matchingButton.interactable = false;
        }
        else
        {
            matchingButtonText.text = "マッチング開始";
            matchingButton.interactable = true;
        }
    }
}
