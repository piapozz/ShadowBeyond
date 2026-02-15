using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderObject : BaseFieldObject
{
    [SerializeField]
    private TextMeshPro defence = null;
    [SerializeField]
    private EvolvePointObject evolvePointObject = null;
    [SerializeField]
    private EvolvePointObject superEvolvePointObject = null;
    [SerializeField]
    private MeshRenderer leaderFrame = null;
    [SerializeField]
    private Material frameMaterial = null;

    public Leader leader { get; private set; } = null;

    /// <summary>
    /// リーダーの情報を設定
    /// </summary>
    /// <param name="setLeader"></param>
    public void Initialize(Leader setLeader)
    {
        leader = setLeader;
        leader.SetGetObjectAction(() => { return this; });
        evolvePointObject.Initialize(false, isLocal);
        superEvolvePointObject.Initialize(true, isLocal);
    }

    public void SetDefenceText(int setDefence)
    {
        defence.text = setDefence.ToString();
    }

    public void PlayEffect(EffectManager.EffectType type, float sec)
    {
        EffectManager.Instance.PlayEffect(type, transform.position, sec);
    }

    public override void SetObjectOutLine(OutLineType type)
    {
        // 現状はマテリアルで見た目変更
        switch (type)
        {
            case OutLineType.None:
                leaderFrame.material = frameMaterial;
                break;
            case OutLineType.Selectable:
                leaderFrame.material = outLineMaterials[((int)OutLineType.Selectable) - 1];
                break;
            case OutLineType.IsSelect:
                leaderFrame.material = outLineMaterials[((int)OutLineType.Selectable) - 1];
                break;
            default: break;
        }
    }
}
