using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameEnum;

public class Field
{
    public List<CardData> _ownFieldCardList = new();
    public List<CardData> _opponentFieldCardList = new();

    const int MAX_FIELD = 5;

    // ===== 共通処理 =====
    private List<CardData> GetAllFieldCards()
    {
        var result = new List<CardData>();
        result.AddRange(_ownFieldCardList);
        result.AddRange(_opponentFieldCardList);
        return result;
    }

    public void OnStartTurn()
    {
        foreach (var card in _ownFieldCardList)
        {
            card.OnStartTurn();
        }
    }

    public void OnEndTurn()
    {
        foreach (var card in _ownFieldCardList)
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
        if (targetList.Count >= MAX_FIELD) return; 
        targetList.Add(card);
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
    public CardData GetFieldCard(int index, bool includeOpponent = false)
    {
        var list = includeOpponent ? GetAllFieldCards() : _ownFieldCardList;
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
    public List<CardData> GetSelectableCards(bool includeOpponent = false)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(c => c.CanBeSelected())
            : _ownFieldCardList.FindAll(c => c.CanBeSelected());
    }

    // アクト可能カード
    public List<CardData> GetActableCards(bool includeOpponent = false)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(c => c.canAct)
            : _ownFieldCardList.FindAll(c => c.canAct);
    }

    // 任意条件カードを取得（複数）
    public List<CardData> GetCards(System.Func<CardData, bool> condition, bool includeOpponent = true)
    {
        return includeOpponent
            ? GetAllFieldCards().FindAll(new System.Predicate<CardData>(condition))
            : _ownFieldCardList.FindAll(new System.Predicate<CardData>(condition));
    }

    // 任意条件カードを1枚（ランダム）
    public CardData GetRandomCard(System.Func<CardData, bool> condition, bool includeOpponent = true)
    {
        var list = GetCards(condition, includeOpponent);
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
        if (!card.activeAbilities.Contains(ability))
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
