using System.Collections;
using System.Collections.Generic;

public abstract class BaseCardAbility
{
    public CardData cardData { get; protected set; }
    public List<KeywordAbilityInstance> keywordAbilities = new List<KeywordAbilityInstance>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();

    public virtual void Initialize(CardData setCard) { }

    // ファンファーレ
    public virtual void Fanfare() { }
    // エンハンス
    public virtual void Enhance() { }
    // 場に出たとき
    public virtual void EnterField() { }
    // 進化時
    public virtual void Evolve() { }
    // 進化した時
    public virtual void AutoEvolve() { }
    // 超進化時
    public virtual void SuperEvolve() { }
    // 超進化した時
    public virtual void AutoSuperEvolve() { }
    // ラストワード
    public virtual void LastWord() { }
    // 場を離れたとき
    public virtual void LeaveField() { }
    // 攻撃時
    public virtual void Attack() { }
    // 交戦時
    public virtual void Combat() { }
    // ダメージを受けて破壊されなかったとき
    public virtual void Damage() { }
    // 捨てられたとき
    public virtual void Discard() { }
    // 攻撃力か体力が場で＋されたとき
    public virtual void Buff() { }
    // 直接召喚されたとき
    public virtual void Invoke() {  }
    // 融合したとき
    public virtual void Fuse() {  }
    // 引いたとき
    public virtual void Draw() {  }
}
