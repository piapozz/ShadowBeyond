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
    private Queue<BaseCardAbility> _timingQueue = null;

    public void Initialize()
    {
        instance = this;
        _timingQueue = new Queue<BaseCardAbility>();
    }

    /// <summary>
    /// トリガーの通知
    /// </summary>
    /// <param name="timing"></param>
    public void AddTrigger(TriggerTiming addTrigger)
    {
        // 誘発する能力の検索
        // キューに追加
        //_timingQueue.Enqueue();
        // 誘発能力発動

    }

    /// <summary>
    /// キューに追加されているトリガーの効果を探し実行
    /// </summary>
    public void ExecuteEffect()
    {
        // キューが空になるまで実行
        while (_timingQueue.Count > 0)
        {
            BaseCardAbility ability = _timingQueue.Dequeue();

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
