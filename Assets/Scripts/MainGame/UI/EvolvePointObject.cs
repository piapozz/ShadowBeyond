using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardObject;

public class EvolvePointObject : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private GameObject evolvePointObject = null;
    [SerializeField]
    private MeshRenderer[] countUI = null;

    private bool isSuperEvolve = false;
    private bool isLocal = false;

    public void Initialize(bool setIsSuperEvolve, bool setIsLocal)
    {
        isSuperEvolve = setIsSuperEvolve;
        isLocal = setIsLocal;
        // 進化ポイントオブジェクトの表示設定
    }

    private void OnMouseDown()
    {
        // 自分のターンか自分の進化権か判定
        if (!BattleManager.instance.IsOwnTurn() || !isLocal) return;
        // 進化権があるか判定
        if (!BattleManager.instance.GetCurrentPlayer().leader.GetCanEvolve(isSuperEvolve)) return;
        // 線を出す
        UIManager.instance.SetLineRenderer(lineRenderer, transform);
        lineRenderer.enabled = true;
    }

    private void OnMouseDrag()
    {
        // 線を出す
        UIManager.instance.SetLineRenderer(lineRenderer, transform);
    }

    private void OnMouseUp()
    {
        lineRenderer.enabled = false;
        Leader leader = BattleManager.instance.GetCurrentPlayer().leader;
        // 進化可能か
        if (!leader.GetCanEvolve(isSuperEvolve)) return;
        // 進化処理
        // マウスの座標からオブジェクトを取得
        BaseFieldObject target = UIManager.instance.GetFieldObject(Input.mousePosition);
        if (target == null) return;
        if (!target.isLocal) return;
        CardObject targetCard = target as CardObject;
        if (targetCard == null || targetCard.currentState != CardState.FIELD || targetCard.cardData.type != GameEnum.CardType.FOLLOWER) return;

        // 自分のフィールドの未進化のフォロワーなら進化可能
        if (targetCard.cardData.isAnyEvolved) return;
        BattleManager.instance.GetCurrentPlayer().leader.SetCanEvolve(false);
        GameEnum.InputType evolveType;
        var ablity = targetCard.cardData.ability;
        if (isSuperEvolve)
        {
            targetCard.SuperEvolveFollower();
            evolveType = GameEnum.InputType.SUPER_EVOLVE;
            countUI[leader.superEvolutionPoint - 1].material = null;
            leader.ConsumeSuperEvolvePoint();
            if(ablity != null) ablity.SuperEvolve(true);
        }
        else
        {
            targetCard.EvolveFollower();
            evolveType = GameEnum.InputType.EVOLVE;
            countUI[leader.evolutionPoint - 1].material = null;
            leader.ConsumeEvolvePoint();
            if (ablity != null) ablity.Evolve(true);
        }
        // 送信
        int fieldIndex = UIManager.instance.GetOwnFieldIndex(targetCard);
        int[] param = new int[1] { fieldIndex };
        BattleManager.instance.SendInputData(evolveType, param);
    }
}
