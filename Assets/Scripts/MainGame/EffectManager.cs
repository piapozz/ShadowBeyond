using CartoonFX;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleManager;

// 攻撃のダメージエフェクト /
// 能力のダメージエフェクト /
// 回復のエフェクト /
// ステータスアップのエフェクト
// ステータスダウンのエフェクト /
// 能力付与のエフェクト /
// 能力解除のエフェクト
// 場に出たとき /
// 破壊されたとき
// 消滅したとき /

public class EffectManager : SystemObject
{
    public static EffectManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> effectPrefab = null;

    [SerializeField]
    private GameObject textEffectPrefab = null;

    public enum EffectType
    {
        AttackDamage,
        AbilityDamage,
        Heal,
        StatusUp,
        StatusDown,
        AbilityAdd,
        OnField,
        OnDestroy,
        OnBanish
    }

    public override async UniTask Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        await UniTask.CompletedTask;
    }

    // エフェクト再生
    public void PlayEffect(EffectType type, Vector3 position, float sec)
    {
        GameObject prefab = effectPrefab[(int)type];
        Instantiate(prefab, position, Quaternion.identity);
    }

    // テキストエフェクト再生
    public void PlayTextEffect(string text, Vector3 position, float sec)
    {
        GameObject prefab = textEffectPrefab;
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.GetComponent<CFXR_ParticleText>().SetText(text);

        CommonModule.WaitAction(sec, () => Destroy(obj));
    }
}
