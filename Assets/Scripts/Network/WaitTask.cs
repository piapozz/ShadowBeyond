using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaitTask : MonoBehaviour
{
    public static WaitTask Instance { get; private set; }

    /// <summary>
    /// タスク情報構造体
    /// </summary>
    private class TaskInfo
    {
        public UniTask Task;
        public CancellationTokenSource Cts;
        public Action Action;
    }

    // 現在進行中のタスクを保持
    private readonly List<TaskInfo> taskList = new List<TaskInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // 完了 or キャンセル済みのタスクを削除
        for (int i = taskList.Count - 1; i >= 0; i--)
        {
            var info = taskList[i];
            if (info.Task.Status.IsCompleted() || info.Cts.IsCancellationRequested)
            {
                info.Cts.Dispose();
                taskList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 指定時間待ってから指定アクションを実行
    /// </summary>
    public int AddTask(Action action, float waitSeconds)
    {
        var cts = new CancellationTokenSource();
        var task = RunTask(action, waitSeconds, cts.Token);

        var info = new TaskInfo()
        {
            Task = task,
            Cts = cts,
            Action = action
        };

        taskList.Add(info);

        // IDとしてインデックスを返す
        return taskList.Count - 1;
    }

    /// <summary>
    /// 実際の非同期処理
    /// </summary>
    private async UniTask RunTask(Action action, float waitSeconds, CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: token);
            if (!token.IsCancellationRequested)
                action?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // キャンセル時はスルー
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WaitTask] タスク中に例外: {ex}");
        }
    }

    /// <summary>
    /// IDを指定してタスクをキャンセル
    /// </summary>
    public void CancelTask(int taskId)
    {
        if (taskId < 0 || taskId >= taskList.Count)
            return;

        taskList[taskId].Cts.Cancel();
    }

    /// <summary>
    /// アクションを指定してタスクをキャンセル
    /// </summary>
    public void CancelTask(Action action)
    {
        for (int i = taskList.Count - 1; i >= 0; i--)
        {
            if (taskList[i].Action == action)
            {
                taskList[i].Cts.Cancel();
                break;
            }
        }
    }

    /// <summary>
    /// すべてのタスクをキャンセル
    /// </summary>
    public void CancelAllTasks()
    {
        foreach (var info in taskList)
        {
            info.Cts.Cancel();
        }
    }
}
