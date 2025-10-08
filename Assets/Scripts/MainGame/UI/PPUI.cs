using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PPUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ppMax = null;
    [SerializeField]
    private TextMeshProUGUI ppCurrent = null;

    public void SetPPText(int setPPMax, int setPPCurrent)
    {
        ppMax.text = setPPMax.ToString();
        ppCurrent.text = setPPCurrent.ToString();
    }
}
