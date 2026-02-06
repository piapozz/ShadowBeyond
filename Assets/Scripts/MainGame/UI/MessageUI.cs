using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;

    public async UniTask MessageText(string text, float sec)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = text;
        float elapsedTime = 0.0f;
        while (elapsedTime < sec)
        {
            elapsedTime += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }
        messageText.gameObject.SetActive(false);
    }
}
