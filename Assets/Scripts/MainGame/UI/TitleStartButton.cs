using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleStartButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonText = null;

    [SerializeField]
    private string sceneName = "Select";

    // Start is called before the first frame update
    void Start()
    {
        buttonText.text = "Tap To Start";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(sceneName);
        }
        // テキストを点滅させる
        float alpha = (Mathf.Sin(Time.time * 3.0f) + 1.0f) / 2.0f;
        buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, alpha);
    }
}
