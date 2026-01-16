using System.Collections;
using System.Collections.Generic;

public abstract class BaseCardAbility
{
    protected CardData cardData;
    public List<KeywordAbilityInstance> keywordAbilities = new List<KeywordAbilityInstance>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();

    public virtual void Initialize() { }

    /// <summary>
    /// アビリティの実行
    /// </summary>
    public void ExecuteAbility()
    {
        for (int i = 0, max = activeAbilities.Count; i < max; i++)
        {
            activeAbilities[i].effect.ExecuteEffect();
        }
    }

    // ファンファーレ
    public virtual ActiveAbility Fanfare() { return null; }
    // エンハンス
    public virtual ActiveAbility Enhance() { return null; }
    // 場に出たとき
    public virtual ActiveAbility EnterField() { return null; }
    // 進化時
    public virtual ActiveAbility Evolve() { return null; }
    // 進化した時
    public virtual ActiveAbility AutoEvolve() { return null; }
    // 超進化時
    public virtual ActiveAbility SuperEvolve() { return null; }
    // 超進化した時
    public virtual ActiveAbility AutoSuperEvolve() { return null; }
    // ラストワード
    public virtual ActiveAbility LastWord() { return null; }
    // 場を離れたとき
    public virtual ActiveAbility LeaveField() { return null; }
    // 攻撃時
    public virtual ActiveAbility Attack() { return null; }
    // 交戦時
    public virtual ActiveAbility Combat() { return null; }
    // ダメージを受けて破壊されなかったとき
    public virtual ActiveAbility Damage() { return null; }
    // 捨てられたとき
    public virtual ActiveAbility Discard() { return null; }
    // 攻撃力か体力が場で＋されたとき
    public virtual ActiveAbility Buff() { return null; }
    // 直接召喚されたとき
    public virtual ActiveAbility Invoke() { return null; }
    // 融合したとき
    public virtual ActiveAbility Fuse() { return null; }
    // 引いたとき
    public virtual ActiveAbility Draw() { return null; }
}
