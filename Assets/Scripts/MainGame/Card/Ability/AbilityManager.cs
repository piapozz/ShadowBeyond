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
        OwnTurnStart,   // ターン開始時
        OwnTurnEnd,     // ターン終了時
        OwnPlay,        // カードをプレイしたとき
        OwnEnterField,  // 場にフォロワーが出たとき
        OwnEvolve,      // フォロワーが進化したとき
        OwnSuperEvolve, // フォロワーが超進化したとき
        OwnDestory,     // フォロワーが破壊されたとき
        OwnLeaveField,  // フォロワーが場を離れたとき
        OwnAttack,      // フォロワーが攻撃した時
        OwnHealLeader,  // リーダーが回復した時
        OwnEngage,      // アミュレットをアクトしたとき
        OwnDraw,        // カードを引いたとき
        OwnMode,        // モードを選んだとき
        OwnFuse,        // 融合したとき
        OwnDamageFollower,      // フォロワーがダメージを受けたとき
        OpponentTurnStart,      // ターン開始時
        OpponentTurnEnd,        // ターン終了時
        OpponentPlay,           // カードをプレイしたとき
        OpponentEnterField,     // 場にフォロワーが出たとき
        OpponentEvolve,         // フォロワーが進化したとき
        OpponentSuperEvolve,    // フォロワーが超進化したとき
        OpponentDestory,        // フォロワーが破壊されたとき
        OpponentLeaveField,     // フォロワーが場を離れたとき
        OpponentAttack,         // フォロワーが攻撃した時
        OpponentHealLeader,     // リーダーが回復した時
        OpponentEngage,         // アミュレットをアクトしたとき
        OpponentDraw,           // カードを引いたとき
        OpponentMode,           // モードを選んだとき
        OpponentFuse,           // 融合したとき
        OpponentDamageFollower, // フォロワーがダメージを受けたとき
    }

    // 実行待機キュー
    private static Queue<ActiveAbility> _waitQueue = new Queue<ActiveAbility>();

    private static Dictionary<TriggerTiming, List<ActiveAbility>> subscribeAbility = new Dictionary<TriggerTiming, List<ActiveAbility>>();

    /// <summary>
    /// トリガーの通知
    /// </summary>
    /// <param name="timing"></param>
    public static void Trigger(TriggerTiming addTrigger, bool isOwnTurn, CardData sourceCard = null)
    {
        if (!subscribeAbility.TryGetValue(addTrigger, out var abilityList))
            return;

        // 誘発する能力の検索、ソートして実行キューに追加
        List<ActiveAbility> activeAbilities = SortActiveAbility(addTrigger, isOwnTurn);
        for (int i = 0, max = activeAbilities.Count; i < max; i++)
        {
            _waitQueue.Enqueue(activeAbilities[i]);
        }
        // 誘発能力発動
        ExecuteEffect(sourceCard);
    }

    /// <summary>
    /// 誘発する能力のソート
    /// </summary>
    /// <param name="sortTiming"></param>
    /// <returns></returns>
    public static List<ActiveAbility> SortActiveAbility(TriggerTiming sortTiming, bool isOwnTurn)
    {
        List<ActiveAbility> sortList = subscribeAbility[sortTiming];
        sortList.Sort((a, b) =>
        {
            // 自ターンなら自分の能力から、相手ターンなら相手の能力からソート
            if (a.isOwn != b.isOwn)
            {
                // 自ターンなら自分を先
                if (isOwnTurn)
                    return a.isOwn ? -1 : 1;
                // 相手ターンなら相手を先
                else
                    return a.isOwn ? 1 : -1;
            }

            // 場所でソート
            return a.zone.CompareTo(b.zone);
        });
        return sortList;
    }

    /// <summary>
    /// キューに登録されている能力の実行
    /// </summary>
    public static void ExecuteEffect(CardData sourceCard)
    {
        // キューが空になるまで実行
        while (_waitQueue.Count > 0)
        {
            // キューから削除
            ActiveAbility ability = _waitQueue.Dequeue();
            // 条件を達成しているか
            if (!ability.condition()) return;
            // 対象を取得
            List<BaseComponent> components;
            if (ability.target == null)
            {
                components = new List<BaseComponent>();
                if (sourceCard == null)
                {
                    components.Add(ability.sourceCard);
                }
                else
                {
                    components.Add(sourceCard);
                }
            }
            else
            {
                components = BattleManager.instance.GetTargetComponent(ability.target, ability.isOwn);
            }
            // 能力発動時に渡すクラス作成
            EffectContext context = new EffectContext(components, ability.sourceCard, ability.isOwn, ability.player, ability.detailCondition);
            // 能力を発動
            ability.effect.ExecuteEffect(context);
        }
    }

    /// <summary>
    /// 能力の登録
    /// </summary>
    /// <param name="timing"></param>
    /// <param name="ability"></param>
    public static void SubscribeAbility(ActiveAbility ability, bool isOwn)
    {
        TriggerTiming timing = ability.timing;
        // 相手のだったらタイミングを反転
        if (!isOwn)
        {
            timing = ReverseTiming(ability.timing);
        }
        if (!subscribeAbility.ContainsKey(timing))
        {
            subscribeAbility[timing] = new List<ActiveAbility>();
        }
        subscribeAbility[timing].Add(ability);
    }

    /// <summary>
    /// 能力の登録解除
    /// </summary>
    /// <param name="ability"></param>
    public static void UnsubscribeAbility(ActiveAbility ability)
    {
        if (subscribeAbility.TryGetValue(ability.timing, out var list))
        {
            list.Remove(ability);

            if (list.Count == 0)
                subscribeAbility.Remove(ability.timing);
        }
    }

    private static TriggerTiming ReverseTiming(TriggerTiming timing)
    {
        switch (timing)
        {
            case TriggerTiming.OwnTurnStart:
                return TriggerTiming.OpponentTurnStart;
            case TriggerTiming.OwnTurnEnd:
                return TriggerTiming.OpponentTurnEnd;
            case TriggerTiming.OwnPlay:
                return TriggerTiming.OpponentPlay;
            case TriggerTiming.OwnEnterField:
                return TriggerTiming.OpponentEnterField;
            case TriggerTiming.OwnEvolve:
                return TriggerTiming.OpponentEvolve;
            case TriggerTiming.OwnSuperEvolve:
                return TriggerTiming.OpponentSuperEvolve;
            case TriggerTiming.OwnDestory:
                return TriggerTiming.OpponentDestory;
            case TriggerTiming.OwnLeaveField:
                return TriggerTiming.OpponentLeaveField;
            case TriggerTiming.OwnAttack:
                return TriggerTiming.OpponentAttack;
            case TriggerTiming.OwnHealLeader:
                return TriggerTiming.OpponentHealLeader;
            case TriggerTiming.OwnEngage:
                return TriggerTiming.OpponentEngage;
            case TriggerTiming.OwnDraw:
                return TriggerTiming.OpponentDraw;
            case TriggerTiming.OwnMode:
                return TriggerTiming.OpponentMode;
            case TriggerTiming.OwnFuse:
                return TriggerTiming.OpponentFuse;
            case TriggerTiming.OwnDamageFollower:
                return TriggerTiming.OpponentDamageFollower;
            case TriggerTiming.OpponentTurnStart:
                return TriggerTiming.OwnTurnStart;
            case TriggerTiming.OpponentTurnEnd:
                return TriggerTiming.OwnTurnEnd;
            case TriggerTiming.OpponentPlay:
                return TriggerTiming.OwnPlay;
            case TriggerTiming.OpponentEnterField:
                return TriggerTiming.OwnEnterField;
            case TriggerTiming.OpponentEvolve:
                return TriggerTiming.OwnEvolve;
            case TriggerTiming.OpponentSuperEvolve:
                return TriggerTiming.OwnSuperEvolve;
            case TriggerTiming.OpponentDestory:
                return TriggerTiming.OwnDestory;
            case TriggerTiming.OpponentLeaveField:
                return TriggerTiming.OwnLeaveField;
            case TriggerTiming.OpponentAttack:
                return TriggerTiming.OwnAttack;
            case TriggerTiming.OpponentHealLeader:
                return TriggerTiming.OwnHealLeader;
            case TriggerTiming.OpponentEngage:
                return TriggerTiming.OwnEngage;
            case TriggerTiming.OpponentDraw:
                return TriggerTiming.OwnDraw;
            case TriggerTiming.OpponentMode:
                return TriggerTiming.OwnMode;
            case TriggerTiming.OpponentFuse:
                return TriggerTiming.OwnFuse;
            case TriggerTiming.OpponentDamageFollower:
                return TriggerTiming.OwnDamageFollower;
            default: return TriggerTiming.None;
        }
    }
}
