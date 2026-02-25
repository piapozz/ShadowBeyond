using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardObject;

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
        RUNE,
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
        NONE,
        OFFICER,    // 兵士
        LUMINOUS,   // ルミナス
        LEVIN,      // レヴィオン
        PIXIE,      // 妖精
        DEPARTED,   // 死者
        EARTH_SIGIL,// 土の印
        MYSTERIA,   // マナリア
        GOLEM,      // ゴーレム
        SHIKIGAMI,  // 式神
        ARTIFACT,   // アーティファクト
        PUPPETRY,   // 人形
        MARINE,     // 海洋
        LOOT,       // 財宝
        ENCROACHER, // アサイラント
        ANATHEMA,   // アナテマ
        COMMANDER,  // 指揮官
        MACHINA,    // 機械
        NATURA,     // 自然
        FESTIVE,    // 宴楽
        HEROIC,     // ヒーロー
        CHESS,      // チェス
        ARMED,      // 武装
        CONDEMNED,  // 八獄
        ALL,        // すべて
        ACADEMIC,   // 学園
    }

    static readonly Dictionary<CardTypeDetail, string> map =
    new Dictionary<CardTypeDetail, string>
    {
        { CardTypeDetail.NONE, "" },
        { CardTypeDetail.OFFICER, "兵士" },
        { CardTypeDetail.LUMINOUS, "ルミナス" },
        { CardTypeDetail.LEVIN, "レヴィオン" },
        { CardTypeDetail.PIXIE, "妖精" },
        { CardTypeDetail.DEPARTED, "死者" },
        { CardTypeDetail.EARTH_SIGIL, "土の印" },
        { CardTypeDetail.MYSTERIA, "マナリア" },
        { CardTypeDetail.GOLEM, "ゴーレム" },
        { CardTypeDetail.SHIKIGAMI, "式神" },
        { CardTypeDetail.ARTIFACT, "アーティファクト" },
        { CardTypeDetail.PUPPETRY, "人形" },
        { CardTypeDetail.MARINE, "海洋" },
        { CardTypeDetail.LOOT, "財宝" },
        { CardTypeDetail.ENCROACHER, "アサイラント" },
        { CardTypeDetail.ANATHEMA, "アナテマ" },
        { CardTypeDetail.COMMANDER, "指揮官" },
        { CardTypeDetail.MACHINA, "機械" },
        { CardTypeDetail.NATURA, "自然" },
        { CardTypeDetail.FESTIVE, "宴楽" },
        { CardTypeDetail.HEROIC, "ヒーロー" },
        { CardTypeDetail.CHESS, "チェス" },
        { CardTypeDetail.ARMED, "武装" },
        { CardTypeDetail.CONDEMNED, "八獄" },
        { CardTypeDetail.ALL, "すべて" },
        { CardTypeDetail.ACADEMIC, "学園" },
    };

    public static string ToText(CardTypeDetail state)
    {
        return map.TryGetValue(state, out var text) ? text : "";
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
        BLOSSOMING_FATE,
        APOCALYPSE_PACT,
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

    public enum KeywordAbility
    {
        Storm,          // 疾走
        Rush,           // 突進
        Ward,           // 守護
        Bane,           // 必殺
        Ambush,         // 潜伏
        Drane,          // ドレイン
        SpellBoost,     // スペルブースト
        Countdown,      // カウントダウン
        Intimidate,     // 威圧
        Aura,           // オーラ
        Barrier,        // バリア
        Enhance,        // エンハンス
        Invoke,         // 直接召喚
        Fuse,           // 融合
        EarthSigle,     // 土の印
        Engage,         // アクト
        SkyboundArt,    // 奥義
        SuperSkyboundArt,// 解放奥義
        NoDestroy,      // 破壊されない
        ClampDamage,    // 固定ダメージ
        MultipulAttack, // 複数回攻撃
        NoAttack,       // 攻撃できない
        OnlySelect,     // これしか選択できない
        NoDamageAbility,// 能力によるダメージを受けない
        NoDamage,       // ダメージを受けない
        NoReciveAbility,// 能力の効果を受けない
        Reducedamege,   // ダメージ軽減
        FreeEvolve,     // 無料進化
        NoEvolve,       // 進化できない
        NoAttackLeader, // リーダーを攻撃できない
        MAX
    }
}
