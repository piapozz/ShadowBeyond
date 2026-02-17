using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameEnum;

public class Field
{
    public List<CardData> _ownFieldCardList = new();
    public List<CardData> _opponentFieldCardList = new();

    public enum FieldType
    {
        OWN,
        OPPONENT,
        ALL
    }

    const int MAX_FIELD = 5;

    // ===== 共通処理 =====
    private List<CardData> GetAllFieldCards()
    {
        var result = new List<CardData>();
        result.AddRange(_ownFieldCardList);
        result.AddRange(_opponentFieldCardList);
        return result;
    }

    public void OnStartTurn(bool isOwn)
    {
        List<CardData> fieldCards = isOwn ? _ownFieldCardList : _opponentFieldCardList;
        foreach (var card in fieldCards)
        {
            card.OnStartTurn();
        }
    }

    public void OnEndTurn(bool isOwn)
    {
        List<CardData> fieldCards = isOwn ? _ownFieldCardList : _opponentFieldCardList;
        foreach (var card in fieldCards)
        {
            card.OnEndTurn();
        }
    }

    // ===== フィールド操作 =====
    // カードを出す
    public void PlayCard(CardData card, int currentIndex)
    {
        if (card == null) return;
        var targetList = currentIndex == 0 ? _ownFieldCardList : _opponentFieldCardList;
        // 手札上限なし
        //if (targetList.Count >= MAX_FIELD) return; 
        targetList.Add(card);
    }

    public void PlayCards(List<CardData> cards, bool isOwn)
    {
        for (int i = 0, max = cards.Count; i < max; i++)
        {
            PlayCard(cards[i], isOwn ? 0 : 1);
        }
    }

    // カードを除外する
    public void RemoveCard(CardData card)
    {
        if (card == null) return;
        _ownFieldCardList.Remove(card);
        _opponentFieldCardList.Remove(card);
    }

    // ===== 条件検索 =====

    // 指定番目のカード
    public CardData GetFieldCard(int index, FieldType fieldType)
    {
        List<CardData> list = null;
        switch (fieldType)
        {
            case FieldType.OWN:
                list = _ownFieldCardList;
                break;
            case FieldType.OPPONENT:
                list = _opponentFieldCardList;
                break;
            case FieldType.ALL:
                list = GetAllFieldCards();
                break;
        }
        if (list == null) return null;

        Debug.Log
            ($"[Field] GetFieldCard index:{index} listCount:{list.Count}");
        if (index < 0 || index >= list.Count) return null;
        return list[index];
    }

    // 相手の指定番目のカード
    public CardData GetOpponentFieldCard(int index)
    {
        Debug.Log($"[Field] GetOpponentFieldCard index:{index} otherFieldCount:{_opponentFieldCardList.Count}");
        if (index < 0 || index >= _opponentFieldCardList.Count) return null;
        return _opponentFieldCardList[index];
    }

    // 選択可能カード
    public List<CardData> GetSelectableCards(FieldType fieldType)
    {
        List<CardData> list = null;
        switch (fieldType)
        {
            case FieldType.OWN:
                list = _ownFieldCardList;
                break;
            case FieldType.OPPONENT:
                list = _opponentFieldCardList;
                break;
            case FieldType.ALL:
                list = GetAllFieldCards();
                break;
        }
        if (list == null) return null;
        return list.FindAll(c => c.CanBeSelected());
    }

    // アクト可能カード
    public List<CardData> GetActableCards(FieldType fieldType)
    {
        List<CardData> list = null;
        switch (fieldType)
        {
            case FieldType.OWN:
                list = _ownFieldCardList;
                break;
            case FieldType.OPPONENT:
                list = _opponentFieldCardList;
                break;
            case FieldType.ALL:
                list = GetAllFieldCards();
                break;
        }
        if (list == null) return null;
        return list.FindAll(c => c.canAct);
    }

    // 任意条件カードを取得（複数）
    public List<CardData> GetCards(System.Func<CardData, bool> condition, FieldType fieldType)
    {
        List<CardData> list = null;
        switch (fieldType)
        {
            case FieldType.OWN:
                list = _ownFieldCardList;
                break;
            case FieldType.OPPONENT:
                list = _opponentFieldCardList;
                break;
            case FieldType.ALL:
                list = GetAllFieldCards();
                break;
        }
        if (list == null) return null;

        return list.FindAll(new System.Predicate<CardData>(condition));
    }

    // 任意条件カードを1枚（ランダム）
    public CardData GetRandomCard(System.Func<CardData, bool> condition, FieldType fieldType)
    {
        List<CardData> list = null;
        switch (fieldType)
        {
            case FieldType.OWN:
                list = _ownFieldCardList;
                break;
            case FieldType.OPPONENT:
                list = _opponentFieldCardList;
                break;
            case FieldType.ALL:
                list = GetAllFieldCards();
                break;
        }
        if (list == null) return null;
        list = list.FindAll(new System.Predicate<CardData>(condition));
        if (list.Count == 0) return null;
        return list[BattleManager.instance.rand.Next(0, list.Count)];
    }

    // 相手の場に守護持ちがいるか
    public bool IsWardOpponentField()
    {
        foreach (var card in _opponentFieldCardList)
        {
            if (card.HaveKeyword(GameEnum.KeywordAbility.Ward))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 自分の場のカード参照インデックス取得
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int GetOwnFieldIndex(CardData card)
    {
        if (card == null) return -1;

        return _ownFieldCardList.IndexOf(card);
    }

    /// <summary>
    /// 相手の場のカード参照インデックス取得
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int GetOpponentFieldIndex(CardData card)
    {
        if (card == null) return -1;

        return _opponentFieldCardList.IndexOf(card);
    }

    public int GetFieldIndex(BaseComponent component)
    {
        bool isOwn = component.GetObject().isLocal;
        if (component is Leader)
        {
            return isOwn ? 0 : 1;
        }

        if (component is CardData)
        {
            if (isOwn)
            {
                int index = GetOwnFieldIndex((CardData)component);
                return index + 2;
            }
            else
            {
                int index = GetOpponentFieldIndex((CardData)component);
                return index + 2 + _ownFieldCardList.Count;
            }
        }
        return -1;
    }

    // ===== 効果系 =====
    // 攻撃力をバフ/デバフ
    public void ModifyAttack(int value, System.Func<CardData, bool> condition = null)
    {
        foreach (var card in GetAllFieldCards())
        {
            if (condition == null || condition(card))
                card.AddStatus(value, 0);
        }
    }

    // 体力をバフ/デバフ
    public void ModifyHealth(int value, System.Func<CardData, bool> condition = null)
    {
        foreach (var card in GetAllFieldCards())
        {
            if (condition == null || condition(card))
                card.AddStatus(0, value);
        }
    }

    // タイプを付与
    public void AddType(CardData card, CardTypeDetail newType)
    {
        if (!card.typeDetail.Contains(newType))
            card.AddTypeDetail(newType);
    }

    // 能力を付与
    public void AddAbility(CardData card, ActiveAbility ability)
    {
        if (!card.ability.activeAbilities.Contains(ability))
            card.AddAbility(ability);
    }

    // 能力を無効化
    public void DisableAllAbilities(CardData card)
    {
        card.ClearAllAbility();
    }

    // ダメージを与える
    public void DamageCard(CardData card, int damage)
    {
        card.DealDamage(damage);
    }

    // 回復する
    public void HealCard(CardData card, int heal)
    {
        card.HealDamage(heal);
    }

    public List<CardData> GetCards(Target.TargetSide targetSide, TargetCondition condition)
    {
        if (targetSide == Target.TargetSide.Own)
        {
            return BattleManager.instance.GetCards(_ownFieldCardList, condition);
        }
        else if (targetSide == Target.TargetSide.Opponent)
        {
            return BattleManager.instance.GetCards(_opponentFieldCardList, condition);
        }
        else if (targetSide == Target.TargetSide.Both)
        {
            List<CardData> cards = new();
            cards.AddRange(_ownFieldCardList);
            cards.AddRange(_opponentFieldCardList);
            return BattleManager.instance.GetCards(cards, condition);
        }
        return null;
    }
}
