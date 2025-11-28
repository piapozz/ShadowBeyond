using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedrawUI : MonoBehaviour
{
    [SerializeField]
    private GameObject redrawCanvas;

    [SerializeField]
    private List<Transform> keepCardPosition;

    [SerializeField]
    private List<Transform> redrawCardPosition;

    [SerializeField]
    private List<GameObject> redrawImage;

    public void StartRedraw()
    {
        // キャンバスを表示
        redrawCanvas.SetActive(true);

        // カードを四枚ドロー
        var player = BattleManager.instance.player[(int)GameEnum.PlayerType.OWN];

        // カードの入力を開始
    }

    public void Redraw()
    {
        // 選択されたカードをデッキに戻す

        // 戻した数ドロー
    }

    public void EndRedraw()
    {
        // キャンバスを非表示
        redrawCanvas.SetActive(false);

        // カードを手札の位置に移動


        // カードのスタイルを変更
    }
}
