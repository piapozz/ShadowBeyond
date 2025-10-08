using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル情報の種類
/// </summary>
public enum BattleStatType
{
    Rally,                      // 連携
    DestroyedFollowers,         // 破壊されたフォロワー
    PlayedSpells,               // プレイしたスペル
    UniqueAmuletsLeft,          // 場を離れたアミュレット名の種類
    UniqueSpellsPlayed,         // プレイしたスペル名の種類
    CardsAddedFromDeck,         // デッキから加えたカードの履歴
    ResonanceCount,             // 共鳴状態になった回数
}

/// <summary>
/// バトル情報の汎用データ
/// </summary>
public class BattleStatValue
{
    public int Count { get; private set; } = 0;
    public HashSet<string> UniqueNames { get; private set; } = new HashSet<string>();
    public List<string> History { get; private set; } = new List<string>();

    public void Add(int value = 1)
    {
        Count += value;
    }

    public void AddUnique(string name)
    {
        if (!string.IsNullOrEmpty(name))
            UniqueNames.Add(name);
    }

    public void AddHistory(string name)
    {
        if (!string.IsNullOrEmpty(name))
            History.Add(name);
    }

    public void Reset()
    {
        Count = 0;
        UniqueNames.Clear();
        History.Clear();
    }
}

/// <summary>
/// リーダー情報
/// </summary>
public class Leader
{
    // 基本情報
    public int playerID { get; private set; }
    public int maxDefense { get; private set; }
    public int currentDefense { get; private set; }
    public int cemetery { get; private set; }
    public int evolutionPoint { get; private set; }
    public int superEvolutionPoint { get; private set; }
    public int maxPlayPoint { get; private set; }
    public int currentPlayPoint { get; private set; }
    public int comboCount { get; private set; }

    // バトル情報
    private Dictionary<BattleStatType, BattleStatValue> battleStats = new Dictionary<BattleStatType, BattleStatValue>();

    public void SetPlayerID(int index)
    {
        playerID = index;
    }

    public Leader(int maxDefense = 20, int maxPP = 0)
    {
        this.maxDefense = maxDefense;
        currentDefense = maxDefense;
        SetMaxPlayPoint(maxPP);
    }

    public void SetMaxDefense(int value)
    {
        maxDefense = value;
        if (currentDefense > maxDefense)
            currentDefense = maxDefense;
    }

    public void SetCurrentDefense(int value)
    {
        currentDefense = Mathf.Clamp(value, 0, maxDefense);
    }

    public void SetMaxPlayPoint(int value)
    {
        maxPlayPoint = value;
        if (currentPlayPoint > maxPlayPoint)
            currentPlayPoint = maxPlayPoint;
        UIManager.instance.UpdatePPUI(playerID, maxPlayPoint, currentPlayPoint);
    }

    public void SetCurrentPlayPoint(int value)
    {
        currentPlayPoint = Mathf.Clamp(value, 0, maxPlayPoint);
        UIManager.instance.UpdatePPUI(playerID, maxPlayPoint, currentPlayPoint);
    }


    private BattleStatValue GetOrCreate(BattleStatType type)
    {
        if (!battleStats.ContainsKey(type))
            battleStats[type] = new BattleStatValue();
        return battleStats[type];
    }

    // --- 数値操作 ---
    public int GetCount(BattleStatType type) => GetOrCreate(type).Count;
    public void AddCount(BattleStatType type, int value = 1) => GetOrCreate(type).Add(value);

    // --- 一意な名前（種類）管理 ---
    public HashSet<string> GetUniqueNames(BattleStatType type) => GetOrCreate(type).UniqueNames;
    public void AddUniqueName(BattleStatType type, string name) => GetOrCreate(type).AddUnique(name);

    // --- 履歴管理 ---
    public List<string> GetHistory(BattleStatType type) => GetOrCreate(type).History;
    public void AddHistory(BattleStatType type, string name) => GetOrCreate(type).AddHistory(name);

    // --- リセット ---
    public void ResetStat(BattleStatType type) => GetOrCreate(type).Reset();
    public void ResetAllStats() => battleStats.Clear();
}


    // バトル情報
    // ニュートラル
    // リーダーの体力の最大値
    // フォロワーが進化した回数
    // 連携の数
    // 破壊されたフォロワーの枚数
    // 破壊された機械フォロワーの枚数
    // 破壊されたナテラの大樹の枚数
    // 融合を持つカード
    // 消費したEPの数
    // 直接召喚を持つカード

    // エルフ
    // プレイした豪風のリノセウスの枚数
    // プレイしたフィルの枚数
    // プレイした密林の森人の枚数
    // 破壊されたコスト1以下のカードの数
    // 破壊されたフェアリーの枚数
    // このターン中に破壊されたフォロワーの数
    // 天香の剣士・ルヴァンで破壊したフォロワーの数
    // 場を離れたアミュレットの名前の種類
    // 場を離れたエルフ・フォロワーの数
    // アクセラレートした回数 
    // このターン中にアクセラレートした回数

    // ロイヤル
    // 破壊されたフォロワーのタイプの種類

    // ウィッチ
    // プレイしたマナリア・カードの枚数
    // プレイしたラピッドファイアの枚数
    // このターン中にプレイしたアミュレットの枚数
    // プレイしたスペルの名前の種類
    // デッキから手札に加えたカードの枚数
    // 消費したスタックの数

    // ドラゴン
    // プレイしたフォロワー以外のカードの枚数
    // プレイした元のコスト7以上のカードの枚数

    // ネクロマンサー
    // プレイしたゾンビドッグの枚数
    // 破壊された機械・カードの数
    // 破壊されたときラストワードを持っていたカードの数
    // 破壊された絶叫の沈黙・ルルナイの数
    // 葬送した回数
    // このターン中に葬送した回数
    // ネクロマンスした値の合計

    // ヴァンパイア
    // デッキから手札に加えたカードの枚数
    // 自分のターン中自分のリーダーがダメージを受けた回数
    // 場に出た永劫の吸血鬼・アルザードの枚数
    // 鏖殺の大悪魔をアクセラレートした回数
    // 酔狂の大悪魔をアクセラレートした回数

    // ビショップ
    // プレイした聖獅子の結晶の枚数
    // プレイしたリモニウムの救済の枚数
    // このターン中に自分のリーダーが回復した回数
    // このターン中に場に出たアミュレットの枚数
    // 破壊されたアミュレットの数
    // 破壊されたとき守護を持っていたフォロワーの数
    // このターン中に消滅した場のカードか自分の手札の枚数
    // 場に出たコスト5以上のフォロワー

    // ネメシス
    // 共鳴状態になった回数
    // このターン自分のフォロワーが与えたダメージ
    // 破壊されたアーティファクト・フォロワーの数
    // 破壊されたアーティファクト・カードの数
    // 破壊されたアーティファクト・カードのコストの合計
    // 破壊されたアーティファクト・カードの名前の種類
    // 破壊されたコスト5以上のフォロワーの数
