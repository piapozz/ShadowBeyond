using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// UIを管理するマネージャー

// 管理するUI
// ・手札UI
// ・ターンUI
// ・デッキUI
// ・フィールドUI
// ・リーダーUI
// ・オプションUI
// ・バトル履歴UI
// ・PPUI
// ・バトル情報UI
public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform UICanvas;

    [SerializeField] private HandUI handUI;
    [SerializeField] private TurnUI turnUI;
    [SerializeField] private DeckUI deckUI;
    [SerializeField] private FieldUI fieldUI;
    [SerializeField] private LeaderUI leaderUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private HistoryUI historyUI;
    [SerializeField] private PPUI ppUI;
    [SerializeField] private InfoUI infoUI;

    private UniTaskCompletionSource _uniTaskCompletionSource = null;

    // Start is called before the first frame update
    void Start()
    {
        // UIを生成
        handUI = Instantiate(handUI, transform);

        // それぞれのUIにコールバック設定
        handUI.AddHandCard(new CardUI());
    }

    // Update is called once per frame
    void Update()
    {
        // 情報UI更新
        // OptionUI更新
        // HistoryUI更新
    }

    // ターン開始
    public void TurnStart()
    {
        // ターンUI更新

        // ドロー処理

        // 手札UI更新

        // PPUI更新

        // リーダーUI更新

        // 手札、フィールド、ボタンの操作可能化
    }

    // ターン終了
    public void TurnEnd()
    {
        // ターンUI更新

        // 手札、フィールド、ボタンの操作不可化
    }

    // Unitaskでなにかがおこるまで待つ関数
    public async UniTask<SendData> InputUI()
    {
        _uniTaskCompletionSource = new UniTaskCompletionSource();



        await _uniTaskCompletionSource.Task;
        return new SendData();
    }
}
