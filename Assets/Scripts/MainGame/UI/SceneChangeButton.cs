using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    public void OnClickChangeScene()
    {
        AudioManager.instance.PlaySE(AudioManager.SEType.BUTTON);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
