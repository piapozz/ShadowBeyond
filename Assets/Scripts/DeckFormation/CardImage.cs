using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

// ドラックアンドドロップでデッキにカードを追加する
// カード画像を表示する
public class CardImage : MonoBehaviour
{
    public int cardId { get; private set; }

    CardLook cardLook = null;

    [SerializeField]
    private List<Material> cardMaterial = null;

    [SerializeField]
    private List<GameObject> cardPrefab = null;

    private GameObject cardObject = null;
    
    private Vector3 startMousePos;

    public void SetCardImage(int id)
    {
        cardId = id;
        if(cardObject != null) cardObject.SetActive(false);
        var cardData = CardMasterUtility.GetCardData(cardId, false);
        switch (cardData.type)
        {
            case GameEnum.CardType.FOLLOWER:
                cardObject = cardPrefab[(int)CardType.FOLLOWER];
                break;
            case GameEnum.CardType.SPELL:
                cardObject = cardPrefab[(int)CardType.SPELL];
                break;
            case GameEnum.CardType.AMULET:
                cardObject = cardPrefab[(int)CardType.AMULET];
                break;
            default: break;
        }
        cardLook = cardObject.GetComponent<CardLook>();
        if (cardLook == null) return;
        // オブジェクト設定
        cardLook.SetCardText(cardData);
        // マテリアル設定
        cardLook.SetCardMaterial(cardMaterial[(int)cardData.rarity]);
        cardObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        // マウスの位置を記録
        Vector3 mousePosition = Input.mousePosition;
        startMousePos = mousePosition;
    }

    private void OnMouseUp()
    {
        // 離された位置が上か下かで処理を分ける
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.y > startMousePos.y + 5.0f)
        {
            // 上に離されたらデッキに追加
            DeckFormationManager.Instance.AddCardToDeck(cardId);
        }
        else
        {
            // 下に離されたらデッキ削除
            DeckFormationManager.Instance.RemoveCardFromDeck(cardId);
        }
    }

}
