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
            AudioManager.instance.PlaySE(AudioManager.SEType.BUTTON);
            if (NetworkManager.Instance.IsConnected())
            {
                // マッチング中止
                NetworkManager.Instance.StopMatchmaking();
                return;
            }
            else
            {
                NetworkManager.Instance.StartMatchmaking();
            }
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
            matchingButtonText.text = "マッチング中止";
            matchingButton.interactable = true;
        }
        else
        {
            matchingButtonText.text = "マッチング開始";
            matchingButton.interactable = true;
        }
    }
}
