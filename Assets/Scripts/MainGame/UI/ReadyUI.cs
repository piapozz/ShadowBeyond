using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ReadyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ownName = null;

    [SerializeField]
    private TextMeshProUGUI ownClassName = null;

    [SerializeField]
    private TextMeshProUGUI opponentName = null;

    [SerializeField]
    private TextMeshProUGUI opponentClassName = null;

    [SerializeField]
    private OrderCardUI OrderCard = null;

    [SerializeField]
    private Transform ownOrderStartRoot = null;

    [SerializeField]
    private Transform opponentOrderStartRoot = null;

    [SerializeField]
    private Transform ownOrderShuffleRoot = null;

    [SerializeField]
    private Transform opponentOrderShuffleRoot = null;

    [SerializeField]
    private Transform ownOrderStatusRoot = null;

    [SerializeField]
    private Transform opponentOrderStatusRoot = null;

    public void Initialize(string setOwnName, string setOwnClassName, string setOpponentName, string setOpponentClassName)
    {
        ownName.text = setOwnName;
        ownClassName.text = setOwnClassName;
        opponentName.text = setOpponentName;
        opponentClassName.text = setOpponentClassName;
    }

    public async UniTask MoveOrderCard(int order)
    {
        // プレハブを生成
        var ownOrderCard = Instantiate(OrderCard);
        ownOrderCard.SetOrder(order);
        var opponentOrderCard = Instantiate(OrderCard);
        opponentOrderCard.SetOrder(1 - order);

        Sequence ownSequence = ownOrderCard.OrderCardAnim(ownOrderStartRoot, ownOrderShuffleRoot, opponentOrderShuffleRoot, ownOrderStatusRoot);
        Sequence opponentSequence = opponentOrderCard.OrderCardAnim(opponentOrderStartRoot, opponentOrderShuffleRoot, ownOrderShuffleRoot, opponentOrderStatusRoot);

        List<Sequence> sequences = new List<Sequence> { ownSequence, opponentSequence };

        UIManager.instance.AddSequence(sequences);

        // シーケンスの完了を待機
        while(!UIManager.instance.IsCompleteAllSequence())
        {
            await UniTask.DelayFrame(1);
        }

        // 一秒待機
        await UniTask.Delay(1000);

        // プレハブを破棄
        Destroy(ownOrderCard.gameObject);
        Destroy(opponentOrderCard.gameObject);

        return;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
