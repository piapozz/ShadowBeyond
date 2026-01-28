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
    private bool isLocal = false;

    public Leader leader { get; private set; } = null;

    /// <summary>
    /// ÉäÅ[É_Å[ÇÃèÓïÒÇê›íË
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
}
