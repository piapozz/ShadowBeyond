using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnum
{
    /// <summary>
    /// カードレアリティ
    /// </summary>
    public enum CardRarity
    {
        INVALID = -1,
        BRONZE,
        SILVER,
        GOLD,
        LEGENDARY,
        GOD,
        MAX
    }

    /// <summary>
    /// リーダークラス
    /// </summary>
    public enum LeaderClass
    {
        INVALID = -1,
        NEUTRAL,
        FOREST,
        SWORD,
        RUNA,
        DRAGON,
        ABYSS,
        HAVEN,
        PORTAL,
        MAX
    }

    /// <summary>
    /// カードの種類
    /// </summary>
    public enum  CardType
    {
        INVALID = -1,
        FOLLOWER,
        SPELL,
        AMULET,
        MAX
    }

    public enum BGM
    {
        TITLE = 0,
        MAIN,
        MAX
    }

    public enum SE
    {

        MAX
    }

    /// <summary>
    /// 同期する種類
    /// </summary>
    public enum SyncType
    {
        INVALID = -1,
        DECK_DATA,
        SEED,
        INPUT,
        MAX
    }

    /// <summary>
    /// 入力の種類
    /// </summary>
    public enum InputType
    {
        INVALID = -1,
        PLAY_CARD,
        ATTACK,
        EVOLVE,
        SUPER_EVOLVE,
        ACT,
        FUSION,
        TURN_END,
        EXTRA_PP,
        OPTION,
        CARD_DETAIL,
        BATTLE_HISTORY,
        MAX
    }
}
