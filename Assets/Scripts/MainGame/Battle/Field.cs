using System.Collections.Generic;
using UnityEngine;
using static GameEnum;

public class Field
{
    public List<CardData> _fieldCardList = new();
    public List<CardData> _otherFieldCardList = new();

    const int MAX_FIELD = 5;

    // ===== 共通処理 =====
    private List<CardData> GetAllFieldCards()
    {
        var result = new List<CardData>();
        result.AddRange(_fieldCardList);
        result.AddRange(_otherFieldCardList);
        return result;
    }

    // ===== フィールド操作 =====
    // カードを出す
    public void PlayCard(CardData card, int currentIndex)
    {
        if (card == null) return;
        var targetList = currentIndex == 0 ? _fieldCardList : _otherFieldCardList;
        if (targetList.Count >= MAX_FIELD) return; 
        targetList.Add(card);
    }

    // カードを除外する
    public void RemoveCard(CardData card)
    {
        if (card == null) return;
        _fieldCardList.Remove(card);
        _otherFieldCardList.Remove(card);
    }

    // ===== 条件検索 =====

    // 指定番目のカード
    public CardData GetFieldCard(int index, bool includeOpponent = false)
    {
        var list = includeOpponent ? GetAllFieldCards() : _fieldCardList;
        if (index < 0 || index >= list.Count) return null;
        return list[index];
    }

    // 相手の指定番目のカード
    public CardData GetOpponentFieldCard(int index)
    {
        if (index < 0 || index >= _otherFieldCardList.Count) return null;
        return _otherFieldCardList[index];
    }

    // 攻撃可能カード
    public List<CardData> GetAttackableCards(bool includeOpponent = false)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(c => c.canAttack)
            : _fieldCardList.FindAll(c => c.canAttack);
    }

    // 選択可能カード
    public List<CardData> GetSelectableCards(bool includeOpponent = false)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(c => c.CanBeSelected())
            : _fieldCardList.FindAll(c => c.CanBeSelected());
    }

    // アクト可能カード
    public List<CardData> GetActableCards(bool includeOpponent = false)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(c => c.canAct)
            : _fieldCardList.FindAll(c => c.canAct);
    }

    // 任意条件カードを取得（複数）
    public List<CardData> GetCards(System.Func<CardData, bool> condition, bool includeOpponent = true)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(new System.Predicate<CardData>(condition))
            : _fieldCardList.FindAll(new System.Predicate<CardData>(condition));
    }

    // 任意条件カードを1枚（ランダム）
    public CardData GetRandomCard(System.Func<CardData, bool> condition, bool includeOpponent = true)
    {
        var list = GetCards(condition, includeOpponent);
        if (list.Count == 0) return null;
        return list[BattleManager.instance.rand.Next(0, list.Count)];
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
    public void AddAbility(CardData card, CardAbility ability)
    {
        if (!card.ability.Contains(ability))
            card.AddAbility(ability);
    }

    // 能力を無効化
    public void DisableAllAbilities(CardData card)
    {
        card.ClearAbility();
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
}
