using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardObject;

public class OrderCardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro orderText = null;

    public void SetOrder(int order)
    {
        if (order == 0)
        {
            orderText.text = "先攻";
        }
        else
        {
            orderText.text = "後攻";
        }
    }

    public Sequence OrderCardAnim(
    Transform startRoot,
    Transform shuffleRoot1,
    Transform shuffleRoot2,
    Transform endRoot)
    {
        Sequence seq = DOTween.Sequence();

        // ---- 0. 初期セット ----
        seq.AppendCallback(() =>
        {
            transform.position = startRoot.position;
            transform.localScale = Vector3.one * 1.5f;
            transform.rotation = startRoot.rotation;
            gameObject.SetActive(true);
        });

        float sec = 0f;
        // ---- 1. startRoot → shuffleRoot1（スケール1.5→1.0 & 移動3秒）----
        sec = 1.5f;
        seq.Append(transform.DOMove(shuffleRoot1.position, sec));
        seq.Join(transform.DOScale(1f, sec));

        // ---- 2. shuffleRoot1 ↔ shuffleRoot2 の往復（移動2秒 & x回転2秒）----
        Sequence pingpong = DOTween.Sequence();

        sec = 0.3f;
        pingpong.Append(transform.DOMove(shuffleRoot2.position, sec));
        pingpong.Join(transform.DORotate(new Vector3(0f, 0f, 360f), sec, RotateMode.LocalAxisAdd));

        pingpong.Append(transform.DOMove(shuffleRoot1.position, sec));
        pingpong.Join(transform.DORotate(new Vector3(0f, 0f, 360f), sec, RotateMode.LocalAxisAdd));

        // 往復回数は必要に応じて変更可能（今回は 1往復 とする）
        pingpong.SetLoops(3, LoopType.Yoyo);

        seq.Append(pingpong);

        // ---- 3. 裏向きにして shuffleRoot1 と shuffleRoot2 の中央へ移動（1秒）----
        sec = 1.0f;
        Vector3 midPos = (shuffleRoot1.position + shuffleRoot2.position) / 2;

        seq.Append(transform.DOMove(midPos, sec));
        seq.Join(transform.DORotate(new Vector3(0f, 0f, 180f), sec)); // 裏向き

        // 待機
        seq.AppendInterval(sec);

        // ---- 4. endRoot へ移動（表向き & スケール1.2 & 時間1秒）----
        sec = 0.5f;
        seq.Append(transform.DOMove(endRoot.position, sec));
        seq.Join(transform.DORotate(endRoot.rotation.eulerAngles, sec));
        seq.Join(transform.DOScale(1.2f, 0.5f));

        return seq;
    }
}
