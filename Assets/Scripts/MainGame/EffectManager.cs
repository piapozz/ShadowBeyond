using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EffectManager : SystemObject
{
    public enum TriggerTiming
    {
        None,

    }

    public struct EffectData
    {
        public TriggerTiming timing;
        public bool isSelf;
        public int param;
    }

    public static EffectManager instance { get; private set; }

    private Queue<EffectData> _triggerQueue = null;

    public override async UniTask Initialize()
    {
        instance = this;
        _triggerQueue = new Queue<EffectData>();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// トリガーの追加
    /// </summary>
    /// <param name="timing"></param>
    public void AddTrigger(EffectData data)
    {
        _triggerQueue.Enqueue(data);
    }

    /// <summary>
    /// キューに追加されているトリガーの効果を探し実行
    /// </summary>
    public void ExecuteEffect()
    {
        // キューが空になるまで実行
        while (_triggerQueue.Count > 0)
        {
            EffectData trigger = _triggerQueue.Dequeue();
            // トリガーの種類を探し一時保存

        }
    }

    /// <summary>
    /// トリガーで誘発する効果の検索
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    public Queue<IEffect> SearchEffectByTrigger(EffectData trigger)
    {
        Queue<IEffect> executeEffects = new Queue<IEffect>();

        

        return executeEffects;
    }
}
