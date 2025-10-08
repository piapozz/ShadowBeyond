using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Transform ownHandRoot = null;
    [SerializeField] private Transform opponentHandRoot = null;
    [SerializeField] private Transform ownDrawRoot = null;
    private bool isAcssessible = false;
    private List<CardObject> ownHandCards = new List<CardObject>();
    private List<CardObject> opponentHandCards = new List<CardObject>();

    private const float HAND_SCALE_X = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAcssessible) return;

        UpdateHandCard();
    }

    // アクセス可能にする
    public void SetAccessible(bool value)
    {
        isAcssessible = value;
    }

    // 自分のカードをドローする
    public void DrawCard(bool isMine, List<CardObject> drawCards, Transform deckRoot)
    {
        List<Sequence> sequenceList = new List<Sequence>();
        for (int i = 0, max = drawCards.Count; i < max; i++)
        {
            CardObject card = drawCards[i];
            card.SetCardState(CardObject.CardState.HAND);
            if (isMine)
            {
                ownHandCards.Add(card);
                // 手札を引くDOTweenのSequenceを取得
                sequenceList.Add(card.DrawOwnCard(deckRoot, ownDrawRoot, ownHandRoot));
            }
            else
            {
                opponentHandCards.Add(card);
                sequenceList.Add(card.DrawOpponentCard(deckRoot, opponentHandRoot));
            }
        }
        // ドローの挙動と整列の挙動を登録
        UIManager.instance.AddSequence(sequenceList);
        UIManager.instance.AddSequence(ArrangeHandCard(isMine));
    }

    // 手札エリアからカードを削除する
    public void RemoveHandCard(bool isMine, CardObject card)
    {
        if (isMine)
        {
            ownHandCards.Remove(card);
            UIManager.instance.AddSequence(ArrangeHandCard(isMine));
        }
        else
        {
            opponentHandCards.Remove(card);
            UIManager.instance.AddSequence(ArrangeHandCard(isMine));
        }
    }

    /// <summary>
    /// 手札のカードを整列する
    /// </summary>
    /// <param name="isMine"></param>
    public List<Sequence> ArrangeHandCard(bool isMine)
    {
        float areaWidth = HAND_SCALE_X;
        float cardWidth = 1.0f;          // 仮のカード幅
        float cardThickness = 0.15f;     // カードの厚み（Y方向のずらし幅）

        List<CardObject> cardList = null;
        if (isMine) cardList = ownHandCards;
        else cardList = opponentHandCards;
        int cardCount = cardList.Count;

        if (cardCount == 0) return null;

        List<Sequence> sequenceList = new List<Sequence>();

        // 「通常の幅」と「エリア内に収めるための幅」を計算
        float maxCardWidth = areaWidth / cardCount;
        float actualWidth = Mathf.Min(cardWidth, maxCardWidth);

        // 全体の横幅を計算（中央揃え用）
        float totalWidth = actualWidth * cardCount;

        for (int i = 0; i < cardCount; i++)
        {
            // 左端基準のX
            float xPosition = -totalWidth / 2 + actualWidth * i + actualWidth / 2;

            // Yは厚み分上げる
            float yPosition = cardThickness * i;

            Sequence arrangeHandSeq = DOTween.Sequence();
            arrangeHandSeq.Append(cardList[i].transform.DOLocalMove(new Vector3(xPosition, yPosition, 0), 0.5f));
            sequenceList.Add(arrangeHandSeq);
        }
        return sequenceList;
    }

    // 手札使用時のコールバック設定
    public void SetUseCardCallback(System.Action useCard)
    {

    }

    // 手札のカードを操作可能にする
    public void UpdateHandCard()
    {
        
    }
}
