using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class BaseCardAbility
{
    public CardData sourceData;
    public List<KeywordAbilityInstance> keywordAbilities = new List<KeywordAbilityInstance>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();
    public Target[] selectTarget = new Target[(int)TargetTiming.Max];

    public enum TargetTiming
    {
        Fanfare = 0,
        Enhance,
        Evolve,
        SuperEvolve,
        Engage,
        Fuse,
        Max
    }

    protected BattleManager.Player GetPlayer(bool isOwn)
    {
        return isOwn ? BattleManager.instance.GetPlayer(0) : BattleManager.instance.GetPlayer(1);
    }

    public virtual void Initialize(CardData setCard) { }

    // ファンファーレ
    public virtual void Fanfare(bool isOwn) { }
    // エンハンス
    public virtual void Enhance(bool isOwn) { }
    // 場に出たとき
    public virtual void EnterField(bool isOwn) { }
    // 進化時
    public virtual void Evolve(bool isOwn) { }
    // 進化した時
    public virtual void AutoEvolve(bool isOwn) { }
    // 超進化時
    public virtual void SuperEvolve(bool isOwn) { }
    // 超進化した時
    public virtual void AutoSuperEvolve(bool isOwn) { }
    // ラストワード
    public virtual void LastWord(bool isOwn) { }
    // 場を離れたとき
    public virtual void LeaveField(bool isOwn) { }
    // 攻撃時
    public virtual void Attack(bool isOwn) { }
    // 交戦時
    public virtual void Combat(bool isOwn) { }
    // ダメージを受けて破壊されなかったとき
    public virtual void Damage(bool isOwn) { }
    // 捨てられたとき
    public virtual void Discard(bool isOwn) { }
    // 攻撃力か体力が場で＋されたとき
    public virtual void Buff(bool isOwn) { }
    // 直接召喚されたとき
    public virtual void Invoke(bool isOwn) {  }
    // 融合したとき
    public virtual void Fuse(bool isOwn) {  }
    // 引いたとき
    public virtual void Draw(bool isOwn) {  }
    // アクト
    public virtual void Engage(bool isOwn, List<BaseComponent> selected = null)
    {
        sourceData.OnAct();
        // 通信
        // 自身のインデックスと選択した対象のインデックスを渡す
        if (isOwn)
        {
            int[] param =
            {
                BattleManager.instance.field.GetOwnFieldIndex(sourceData), // 場の何番目か
            };
            // 選択が含まれるなら、選択したコンポーネントのインデックスを渡す
            if (selected != null)
            {
                for (int i = 0, max = selected.Count; i < max; i++)
                {
                    int index = BattleManager.instance.field.GetFieldIndex(selected[i]);
                    if (index < 0) break;
                    param[i + 1] = index;
                }
            }
            BattleManager.instance.SendInputData(GameEnum.InputType.ACT, param);
        }
    }
    // スペルブースト
    public virtual void SpellBoost(bool isOwn) { }
}
