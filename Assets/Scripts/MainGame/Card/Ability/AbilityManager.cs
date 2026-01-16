using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AbilityManager
{
    public enum TriggerTiming
    {
        Play,               //自分がカードをプレイしたとき
        OwnEnterField,      //自分の場にフォロワーが出たとき
        OpponentEnterField, //相手の場にフォロワーが出たとき
        Evolve,             //自分のフォロワーが進化したとき
        OwnSuperEvolve,     //自分のフォロワーが超進化したとき
        OpponentSuperEvolve,//相手のフォロワーが超進化したとき
        Destory,            //自分のフォロワーが破壊されたとき
        LeaveField,         //自分のフォロワーが場を離れたとき
        OwnAttack,          //自分のフォロワーが攻撃した時
        OpponentAttack,     //相手のフォロワーが攻撃した時
        HealLeader,         //自分のリーダーが回復した時
        Engage,             //自分のアミュレットをアクトしたとき
        Draw,               //自分がカードを引いたとき
        Mode,               //自分がモードを選んだとき
        Fuse,               //自分が融合したとき
        DamageFollower,     //自分のフォロワーがダメージを受けたとき
    }

    public static AbilityManager instance { get; private set; }

    // 実行待機キュー
    private Queue<BaseCardAbility> _abilityQueue = null;

    public void Initialize()
    {
        instance = this;
        _abilityQueue = new Queue<BaseCardAbility>();
    }

    /// <summary>
    /// トリガーの追加
    /// </summary>
    /// <param name="timing"></param>
    public void AddTrigger(BaseCardAbility data)
    {
        _abilityQueue.Enqueue(data);
    }

    /// <summary>
    /// キューに追加されているトリガーの効果を探し実行
    /// </summary>
    public void ExecuteEffect()
    {
        // キューが空になるまで実行
        while (_abilityQueue.Count > 0)
        {
            BaseCardAbility ability = _abilityQueue.Dequeue();
            // トリガーの種類を探し一時保存
            ability.ExecuteAbility();
        }
    }

    /// <summary>
    /// トリガーで誘発する効果の検索
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    public Queue<ActiveAbility> SearchEffectByTrigger(BaseCardAbility trigger)
    {
        Queue<ActiveAbility> executeEffects = new Queue<ActiveAbility>();

        

        return executeEffects;
    }
}
