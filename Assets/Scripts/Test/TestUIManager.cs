using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardObject;
using static CommonModule;

public class TestUIManager : MonoBehaviour
{
    [SerializeField] private Transform UICanvas;

    [SerializeField] private ReadyUI readyUI;

    public enum UIState
    {
        INVALID = -1,
        DEFAULT,
        ATTACK,
        MAX
    }

    public static TestUIManager instance { get; private set; }

    public UIState state { get; private set; } = UIState.DEFAULT;
    private Queue<List<Sequence>> uiSequence = null;
    private List<Sequence> currentSequenceList = null;
    private List<CardObject> poolCardObject = null;
    private Camera mainCamera = null;

    private const int POOL_CARD_NUM = 30;


    public async void Start()
    {
        await Initialize();
        await PlayStartBattleSequence(1);
    }

    public async UniTask Initialize()
    {
        // DOTween初期化
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;

        instance = this;
        mainCamera = Camera.main;
        uiSequence = new Queue<List<Sequence>>();
        currentSequenceList = new List<Sequence>();
    }

    private void Update()
    {
        // UIシーケンス処理
        UISequence();
    }

    private void UISequence()
    {
        // UIシーケンス処理
        if (IsCompleteAllSequence()) return;
        // シーケンスがたまっていて、現在のシーケンスが終了していたら次のシーケンスを再生
        if (IsCompleteCurrentSequence())
        {
            currentSequenceList = uiSequence.Dequeue();
            for (int i = 0, max = currentSequenceList.Count; i < max; i++)
            {
                currentSequenceList[i].Play();
            }
        }
    }

    public BaseFieldObject GetFieldObject(Vector2 screenPos)
    {
        // カードの詳細画面表示
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        GameObject hitObject = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            hitObject = hit.collider.gameObject;
        }
        if (hitObject == null) return null;
        BaseFieldObject target = hitObject.GetComponent<BaseFieldObject>();
        return target;
    }

    private bool IsCompleteCurrentSequence()
    {
        for (int i = 0, max = currentSequenceList.Count; i < max; i++)
        {
            if (!currentSequenceList[i].IsActive()) continue;

            if (currentSequenceList[i].IsActive() && !currentSequenceList[i].IsComplete()) return false;
        }
        return true;
    }

    public void AddSequence(List<Sequence> addSequences)
    {
        if (addSequences == null) return;
        uiSequence.Enqueue(addSequences);
    }

    public bool IsCompleteAllSequence()
    {
        return uiSequence.Count == 0;
    }

    // バトル開始時演出
    public async UniTask PlayStartBattleSequence(int currntPlayer)
    {
        // TODO: バトル開始時演出

        // キャラだし
        var readiUIobj = Instantiate(readyUI, UICanvas);
        readiUIobj.Initialize("Player", "Warrior", "Opponent", "Mage");


        // 順番決め演出
        await readiUIobj.MoveOrderCard(currntPlayer);

        // フィールドのセット
    }
}