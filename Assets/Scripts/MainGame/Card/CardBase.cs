using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameEnum;

/// <summary>
/// カードの基底クラス
/// </summary>
public class CardBase
{
    public struct CardData
    {
        public string m_ID;
        public Class m_class;
        public CardRarity m_rarity;
        public CardType m_type;
        public string m_name;
        public int m_cost;
        public FollowerStatus m_status;
        public List<CardAbility> m_ability;

        public CardData(string setID, Class setClass, CardRarity setRarity, CardType setType, string setName, int setCost, FollowerStatus setStatus, List<CardAbility> setAbility)
        {
            m_ID = setID;
            m_class = setClass;
            m_rarity = setRarity;
            m_type = setType;
            m_name = setName;
            m_cost = setCost;
            m_status = setStatus;
            m_ability = setAbility;
        }
    }

    public struct FollowerStatus
    {
        public int m_attack;
        public int m_health;
    }

    public CardData _data { get; private set; }
    public int _damage { get; private set; }
    public List<FollowerStatus> _addStatus { get; private set; }
    public bool _canAttack { get; private set; }

    /// <summary>
    /// データの設定
    /// </summary>
    /// <param name="setData"></param>
    public void SetData(CardData setData)
    {
        _data = setData;
    }

    /// <summary>
    /// データクリア
    /// </summary>
    public void ClearData()
    {
        _data = new CardData();
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage"></param>
    public void DealDamage(int damage)
    {
        _damage += damage;
    }

    /// <summary>
    /// 現在のステータスを取得
    /// </summary>
    /// <returns></returns>
    public FollowerStatus GetCurrentStatus()
    {
        FollowerStatus currentStatus = _data.m_status;
        for (int i = 0, max = _addStatus.Count; i < max; i++)
        {
            currentStatus.m_attack += _addStatus[i].m_attack;
            currentStatus.m_health += _addStatus[i].m_health;
        }
        currentStatus.m_health -= _damage;

        return currentStatus;
    }
}
