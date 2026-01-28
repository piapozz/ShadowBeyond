using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AbilityManager
{
    public enum TriggerTiming
    {
        None = 0,
        OwnTurnStart,       //自分のターン開始時
        OwnTurnEnd,         //自分のターン終了時
        OpponentTurnStart,  //相手のターン開始時
        OpponentTurnEnd,    //相手のターン終了時
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
    private Queue<ActiveAbility> _timingQueue = null;

    private Dictionary<TriggerTiming, List<ActiveAbility>> subscribeAbility;

    public void Initialize()
    {
        instance = this;
        _timingQueue = new Queue<ActiveAbility>();
        subscribeAbility = new Dictionary<TriggerTiming, List<ActiveAbility>>();
    }

    /// <summary>
    /// トリガーの通知
    /// </summary>
    /// <param name="timing"></param>
    public void Trigger(TriggerTiming addTrigger)
    {
        // 誘発する能力の検索、ソートして実行キューに追加
        List<ActiveAbility> activeAbilities = SortActiveAbility(addTrigger);
        for (int i = 0, max = activeAbilities.Count; i < max; i++)
        {
            _timingQueue.Enqueue(activeAbilities[i]);
        }
        // 誘発能力発動
        ExecuteEffect();
    }

    public List<ActiveAbility> SortActiveAbility(TriggerTiming sortTiming)
    {
        List<ActiveAbility> sortList = subscribeAbility[sortTiming];
        sortList.Sort((a, b) =>
        {
            return a.zone.CompareTo(b.zone);
        });
        return sortList;
    }

    /// <summary>
    /// キューに登録されている能力の実行
    /// </summary>
    public void ExecuteEffect()
    {
        // キューが空になるまで実行
        while (_timingQueue.Count > 0)
        {
            ActiveAbility ability = _timingQueue.Dequeue();
            bool isOwn = ability.sourceCard.GetObject().isLocal;
            List<BaseComponent> components = BattleManager.instance.GetTargetCard(ability.target, isOwn);
            ability.effect.ExecuteEffect(components);
        }
    }

    /// <summary>
    /// 能力の登録
    /// </summary>
    /// <param name="timing"></param>
    /// <param name="ability"></param>
    public void SubscribeAbility(TriggerTiming timing, ActiveAbility ability)
    {
        if (!subscribeAbility.ContainsKey(timing))
        {
            subscribeAbility[timing] = new List<ActiveAbility>();
        }
        subscribeAbility[timing].Add(ability);
    }
}
