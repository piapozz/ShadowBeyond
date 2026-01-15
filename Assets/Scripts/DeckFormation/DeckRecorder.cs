using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// プレイヤーのデッキ構成をJSON形式で保存・読み込みするクラス。
/// 各デッキはカードIDのリストとして保持され、保存時にIDと枚数に圧縮されます。
/// </summary>
public class DeckRecorder : MonoBehaviour
{
    public static DeckRecorder Instance;

    // JSONファイルの保存先
    private string jsonFilePath = "Assets/Resources/DeckRecords/DeckRecords.json";

    // メモリ上の全デッキ情報
    private DeckCollection deckCollection = new DeckCollection();

    // ------------------------
    // 🔹 データ構造定義
    // ------------------------

    /// <summary>
    /// 1つのデッキ（カードIDの一覧）
    /// </summary>
    [System.Serializable]
    public class DeckRecord
    {
        public List<int> cardIds = new List<int>();
    }

    /// <summary>
    /// 保存用：カードIDとその枚数
    /// </summary>
    [System.Serializable]
    public class CardCountEntry
    {
        public int cardId;
        public int count;
        public CardCountEntry(int id, int count)
        {
            cardId = id;
            this.count = count;
        }
    }

    /// <summary>
    /// 保存用：1つのデッキ（IDと枚数のリスト）
    /// </summary>
    [System.Serializable]
    public class DeckSaveEntry
    {
        public List<CardCountEntry> cards = new List<CardCountEntry>();
    }

    /// <summary>
    /// 実行時管理用：全デッキのリスト
    /// </summary>
    [System.Serializable]
    public class DeckCollection
    {
        public List<DeckRecord> decks = new List<DeckRecord>();
    }

    /// <summary>
    /// 保存用：全デッキのリスト
    /// </summary>
    [System.Serializable]
    public class DeckSaveData
    {
        public List<DeckSaveEntry> decks = new List<DeckSaveEntry>();
    }

    public List<int> GetCurrentDeck()
    {
        return deckCollection.decks[0].cardIds;
    }

    // ------------------------
    // 🔹 初期化
    // ------------------------

    private void Awake()
    {
        // Singletonパターン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // フォルダが存在しない場合は作成
        string directory = Path.GetDirectoryName(jsonFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // JSONファイルが存在しない場合は初期データで作成
        if (!File.Exists(jsonFilePath))
        {
            var initSaveData = new DeckSaveData();
            initSaveData.decks = new List<DeckSaveEntry>();
            initSaveData.decks.Add(new DeckSaveEntry());
            initSaveData.decks[0].cards = new List<CardCountEntry>();
            // カードID 2 を40枚
            initSaveData.decks[0].cards.Add(new CardCountEntry(2, 40)); 

            string json = JsonUtility.ToJson(initSaveData, true);
            File.WriteAllText(jsonFilePath, json);
        }

        LoadAllDecks();
    }

    // ------------------------
    // 🔹 デッキの読み込み
    // ------------------------

    private void LoadAllDecks()
    {
        string jsonText = File.ReadAllText(jsonFilePath);
        var saveData = JsonUtility.FromJson<DeckSaveData>(jsonText);

        deckCollection.decks.Clear();

        if (saveData != null && saveData.decks != null)
        {
            foreach (var entry in saveData.decks)
            {
                DeckRecord record = ConvertToDeckRecord(entry);
                deckCollection.decks.Add(record);
            }
        }

        Debug.Log($"[DeckRecorder] Loaded {deckCollection.decks.Count} decks.");
    }

    // ------------------------
    // 🔹 デッキの追加・保存
    // ------------------------

    /// <summary>
    /// 新しいデッキを登録してJSONに保存する
    /// </summary>
    public void SaveNewDeck(List<int> cardIdList)
    {
        if (deckCollection == null)
            deckCollection = new DeckCollection();

        DeckRecord newDeck = new DeckRecord();
        newDeck.cardIds = new List<int>(cardIdList);
        deckCollection.decks.Clear();
        deckCollection.decks.Add(newDeck);

        SaveAllDecksToJson();

        Debug.Log($"[DeckRecorder] Saved deck #{deckCollection.decks.Count}");
    }

    /// <summary>
    /// デッキの削除
    /// </summary>
    /// <param name="DeckId"></param>
    public void RemoveDeck(int deckId)
    {
        if(deckCollection == null)
            return;
        if (deckCollection.decks.Count <= deckId)
            return;
        deckCollection.decks.RemoveAt(deckId);
    }

    /// <summary>
    /// メモリ上の全デッキをJSONに書き出す
    /// </summary>
    private void SaveAllDecksToJson()
    {
        DeckSaveData saveData = new DeckSaveData();

        foreach (DeckRecord record in deckCollection.decks)
        {
            DeckSaveEntry entry = ConvertToSaveEntry(record);
            saveData.decks.Add(entry);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(jsonFilePath, json);
    }

    // ------------------------
    // 🔹 変換処理
    // ------------------------

    /// <summary>
    /// デッキデータ → 保存形式（IDと枚数）
    /// </summary>
    private DeckSaveEntry ConvertToSaveEntry(DeckRecord record)
    {
        var countDict = new Dictionary<int, int>();
        foreach (int id in record.cardIds)
        {
            if (countDict.ContainsKey(id))
                countDict[id]++;
            else
                countDict[id] = 1;
        }

        var entry = new DeckSaveEntry();
        foreach (var kvp in countDict)
        {
            entry.cards.Add(new CardCountEntry(kvp.Key, kvp.Value));
        }

        return entry;
    }

    /// <summary>
    /// 保存形式 → デッキデータ（枚数を展開）
    /// </summary>
    private DeckRecord ConvertToDeckRecord(DeckSaveEntry entry)
    {
        var record = new DeckRecord();

        foreach (var card in entry.cards)
        {
            for (int i = 0; i < card.count; i++)
            {
                record.cardIds.Add(card.cardId);
            }
        }

        return record;
    }
}
