using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ターン進行
// バトル全体の管理
public class BattleManager : SystemObject
{
    const int PLAYPOINT_MAX = 10;

    public enum BattleState
    {
        INVALID = -1,
        START_BATTLE,
        START_TURN,
        MAIN_TURN,
        END_TURN,
        END_BATTLE
    }

    public struct Player
    {
        private Hand _hand;
        private Leader _leader;
        private Deck _deck;

        int turn;
        int playPoint;
        bool extraPoint;

        public void Init()
        {
            turn = 0;
            playPoint = 0;
            extraPoint = false;
            _hand = new Hand();
            _deck = new Deck();
        }

        public void SetLeader(Leader leader)
        {
            _leader = leader;
        }

        public void nextTurn()
        {
            turn++;
            if (playPoint < PLAYPOINT_MAX) playPoint++;
            _leader.SetMaxPlayPoint(playPoint);
        }
    }

    private Field _field;

    private Player[] _player;

    private int _currentPlayerIndex = 0;

    private BattleState _currentState = BattleState.INVALID;

    public void Init()
    {
        _player = new Player[2];
        _player[0].Init();
        _player[1].Init();
        _field = new Field();
        _player[0].SetLeader(new Leader(0, 0));
        _player[1].SetLeader(new Leader(1, 0));
        _currentState = BattleState.START_BATTLE;
    }

    public void Update()
    {
        switch(_currentState)
        {
            case BattleState.START_BATTLE:
                StartBattle();
                _currentState = BattleState.START_TURN;
                break;
            case BattleState.START_TURN:
                StartTurn();
                _currentState = BattleState.MAIN_TURN;
                break;
            case BattleState.MAIN_TURN:
                // メインフェイズ処理
                // 条件を満たしたらターン終了へ
                MainTurn();
                break;
            case BattleState.END_TURN:
                EndTurn();
                _currentState = BattleState.START_TURN;
                break;
            case BattleState.END_BATTLE:
                // バトル終了処理
                EndBattle();
                break;
            default:
                break;
        }
    }

    public void StartBattle()
    {
        // バトル開始処理

        // 先攻後攻決める
        int first = Random.Range(0, 2);
        _currentPlayerIndex = first;

        // UIだす
    }

    public void StartTurn()
    {
        // ターン開始処理
        _player[_currentPlayerIndex].nextTurn();

        // ドロー
    }

    public void MainTurn()
    {
        // メインフェイズ処理
        _currentState = BattleState.END_TURN; // 仮でターン終了へ
    }

    public void EndTurn()
    {
        // ターン終了処理
        _currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
    }

    public void EndBattle()
    {
        // バトル終了処理
    }

    public BattleState GetCurrentState() { return _currentState; }

}
