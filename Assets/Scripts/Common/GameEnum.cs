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
        SHADOW,
        BLOOD,
        HAVEN,
        PORTAL,
        ABYSS,
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
    /// <summary>
    /// カードのタイプ
    /// </summary>
    public enum CardTypeDetail
    {
        INVALID = -1,
        MACHINA,    // 機械
        NATURA,     // 自然
        FESTIVE,    // 宴楽
        CONDEMNED,  // 八獄
        ACADEMIC,   // 学園
        COMMANDER,  // 指揮官
        OFFICER,    // 兵士
        LOOT,       // 財宝
        LEVIN,      // レヴィオン
        HEROIC,     // ヒーロー
        EARTH_SIGIL,// 土の印
        MYSTERIA,   // マナリア
        CHESS,      // チェス
        ARMED,      // 武装
        ARTIFACT,   // アーティファクト
        ALL,        // すべて
        DEPARTED,   // 死者
        PIXIE       // 妖精
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
        ATTACK_FOLLOWER,
        ATTACK_LEADER,
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

    /// <summary>
    /// プレイヤーの種類
    /// </summary>
    public enum PlayerType
    {
        OWN,
        OPPONENT,

        MAX
    }

    public enum PackType
    {
        INVALID = -1,
        BASIC_BEYOND,
        LEGENDS_RISE,
        INFINITY_EVOLVED,
        HEIRS_OF_THE_OMEN,
        SKYBOUND_DRAGONS,
        BASIC,
        CLC,
        DRK,
        ROB,
        TOG,
        WLD,
        SFL,
        CGS,
        DBN,
        BOS,
        OOT,
        ALT,
        STR,
        ROG,
        VEC,
        UCL,
        WUP,
        FOH,
        SOR,
        ETA,
        DOV,
        RSC,
        DOC,
        OOS,
        EOP,
        RGW,
        CDB,
        EAA,
        AOA,
        HOR,
        ORS,
        RSL,
        HOS,
        MAX
    }
}
