using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

/// <summary>
/// カードオブジェクトにアタッチするクラス
/// </summary>
public class CardObject : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cardPrefab = null;
    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private List<Material> cardMaterial = null;

    public enum CardObjectType
    {
        INVALID = -1,
        HAND_FOLLOWER,
        HAND_SPELL,
        HAND_AMULET,
        FIELD_FOLLOWER,
        EVOLVE_FOLLOWER,
        SUPWER_EVOLVE_FOLLOWER,
        FIELD_AMULET,
        MAX
    }

    public enum CardState
    {
        INVALID = -1,
        HAND,
        FIELD,
        MAX
    }

    private CardState currentState = CardState.HAND;

    // カードクラスの参照
    private CardData cardData = null;
    private GameObject[] cardObject = new GameObject[(int)CardState.MAX];
    private Camera mainCamera = null;

    // カードをドラッグした時の高さオフセット
    private const float OFFSET_Y = 1.0f;
    // 攻撃線の制御点数
    private const int LINE_CONTROL_POINT_NUM = 20;
    // 攻撃線の高さ
    private const float LINE_HEIGHT = 5.0f;
    // 攻撃線のずらし幅
    private const float LINE_OFFSET = 2.0f;

    public void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// カードクラスを渡す
    /// </summary>
    public void SetCardData(CardData setCard)
    {
        cardData = setCard;
        SetCardLook();
    }

    private void OnMouseDown()
    {
        switch (currentState)
        {
            case CardState.HAND:
                break;
            case CardState.FIELD:
                // 攻撃の線を出す
                lineRenderer.enabled = true;
                break;
        }
    }

    private void OnMouseDrag()
    {
        switch (currentState)
        {
            case CardState.HAND:
                // カードの位置更新
                Vector3 position = GetMouseWorldPosition(transform, mainCamera);
                position.y = OFFSET_Y;
                transform.position = position;
                break;
            case CardState.FIELD:
                // 攻撃の線を出す
                SetLineRenderer();
                break;
        }
    }

    private void OnMouseUp()
    {
        switch (currentState)
        {
            case CardState.HAND:
                // オブジェクトをUIにセット
                UIManager.instance.SetCardDrop(this);
                break;
            case CardState.FIELD:
                lineRenderer.enabled = false;
                // 攻撃処理
                Attack();
                break;
        }
    }

    /// <summary>
    /// LineRendererの設定
    /// </summary>
    private void SetLineRenderer()
    {
        // 制御点の総数
        int totalPointNum = LINE_CONTROL_POINT_NUM + 2;
        // 始点終点
        Vector3 startPoint = transform.position;
        Vector3 endPoint = GetMouseWorldPosition(transform, mainCamera);
        // 中間点を曲線が見えるようにずらす
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 midPoint = (startPoint + endPoint) / 2 + Vector3.up * LINE_HEIGHT + cameraUp * LINE_OFFSET;

        // 始点登録
        lineRenderer.SetPosition(0, startPoint);
        for (int i = 1; i <= LINE_CONTROL_POINT_NUM; i++)
        {
            float t = (float)i / (float)(totalPointNum - 1);
            Vector3 point = GetBezierCurve2(startPoint, endPoint, midPoint, t);
            lineRenderer.SetPosition(i, point);
        }
        // 終点登録
        lineRenderer.SetPosition(totalPointNum - 1, endPoint);

        // 太さ設定
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        // マウスの座標からオブジェクトを取得
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        CardObject target = null;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;
            target = hitObject.GetComponent<CardObject>();
        }
        if (target == null) return;
        // 攻撃可能オブジェクトか判定(フィールドに出ている敵フォロワーか敵リーダー)

        // 攻撃処理を依頼

    }

    /// <summary>
    /// カードの状態の設定
    /// </summary>
    /// <param name="state"></param>
    public void SetCardState(CardState state)
    {
        currentState = state;
        // オブジェクトの切り替え
        switch (currentState)
        {
            case CardState.HAND:
                cardObject[(int)CardState.HAND].SetActive(true);
                break;
            case CardState.FIELD:
                cardObject[(int)CardState.HAND].SetActive(false);
                cardObject[(int)CardState.FIELD].SetActive(true);
                break;
        }
    }

    /// <summary>
    /// カードの見た目の適用
    /// </summary>
    public void SetCardLook()
    {
        // オブジェクト設定
        switch (cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_FOLLOWER];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.FIELD_FOLLOWER];
                break;
            case GameEnum.CardType.SPELL:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_SPELL];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.HAND_SPELL];
                break;
            case GameEnum.CardType.AMULET:
                cardObject[(int)CardState.HAND] = cardPrefab[(int)CardObjectType.HAND_AMULET];
                cardObject[(int)CardState.FIELD] = cardPrefab[(int)CardObjectType.FIELD_AMULET];
                break;
            default: break;
        }
        // 手札オブジェクト設定
        // テキスト設定
        CardLook handLook = cardObject[(int)CardState.HAND].GetComponent<CardLook>();
        if (handLook == null) return;
        handLook.SetCardText(cardData);
        // マテリアル設定
        handLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);

        // フィールドオブジェクト設定
        // テキスト設定
        CardLook fieldLook = cardObject[(int)CardState.FIELD].GetComponent<CardLook>();
        if (handLook == null) return;
        fieldLook.SetCardText(cardData);
        // マテリアル設定
        fieldLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);
    }

    public void PlayCard()
    {

    }

    public void EvolveFollower()
    {
        // 進化前挙動


        // 進化後挙動

    }

    public void SuperEvolveFollower()
    {
        // 進化前挙動


        // 進化後挙動

    }
}
