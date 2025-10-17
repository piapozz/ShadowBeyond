using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ターン進行
// バトル全体の管理
public class BattleManager : SystemObject
{
    public static BattleManager instance { get; private set; } = null;

    const int PLAYPOINT_MAX = 10;
    const int FIELD_CARD_MAX = 5;
    public int localPlayerIndex = -1;
    private int seed = 0;

    public System.Random rand { get; private set; } // シード保持用

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
        public Hand hand;
        public Leader leader;
        public Deck deck;

        int turn;
        bool extraPoint;

        public void Init()
        {
            turn = 0;
            extraPoint = false;
            hand = new Hand();
            deck = new Deck();
            List<CardData> deckList = new List<CardData>(40);
            for (int i = 0; i < 40; i++)
            {
                deckList.Add(CardMasterUtility.GetRandomCardData());
            }

            deck.Init(deckList);
            hand.Init(new List<CardData>());
        }

        public void SetLeader(Leader leader)
        {
            this.leader = leader;
        }

        public void NextTurn()
        {
            turn++;
            int playPoint = leader.maxPlayPoint;
            if (playPoint < PLAYPOINT_MAX) playPoint++;
            leader.SetMaxPlayPoint(playPoint);
            leader.SetCurrentPlayPoint(playPoint);
            deck.DrawDeck(1);
        }

        public void SetPlayerID(int index)
        {
            hand.SetPlayerID(index);
            deck.SetPlayerID(index);
            leader.SetPlayerID(index);
        }
    }
    public Player[] player { get; private set; }

    public int currentPlayerIndex { get; private set; }

    public Field field { get; private set; }

    private BattleState currentState = BattleState.INVALID;

    public override async UniTask Initialize()
    {
        instance = this;
       
        currentState = BattleState.START_BATTLE;
        localPlayerIndex = NetworkManager.Instance.localPlayerId;
        await UniTask.CompletedTask;
    }

    public void Update()
    {
        switch(currentState)
        {
            case BattleState.START_BATTLE:
                StartBattle();
                break;
            case BattleState.START_TURN:
                StartTurn();
                currentState = BattleState.MAIN_TURN;
                break;
            case BattleState.MAIN_TURN:
                // メインフェイズ処理
                // 条件を満たしたらターン終了へ
                MainTurn();
                break;
            case BattleState.END_TURN:
                EndTurn();
                currentState = BattleState.START_TURN;
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
        // プレイヤー数が揃っていなければ開始しない
        if (NetworkManager.Instance.GetActivePlayerCount() < 2)
        {
            return;
        }

        // シード値を決定
        if (localPlayerIndex == 1)
        {
            // シード値生成
            seed = Random.Range(int.MinValue, int.MaxValue);
            int result = NetworkManager.Instance.SendSeedData(seed);

            if (result != 0)
            {
                Debug.Log("[Battle] ❌ シード値の送信に失敗しました");
                return;
            }
        }
        else
        {
            Debug.Log("[Battle] 🌱 シード値を待機中...");
            if (!NetworkManager.Instance.seedReceived) return;

            seed = NetworkManager.Instance.GetReceivedSeed();
            Debug.Log($"[Battle] 🎲 シード値を取得: {seed}");
        }

        rand = new System.Random(seed);

        player = new Player[(int)GameEnum.PlayerType.MAX];

        field = new Field();

        // 先攻後攻決める
        int first = rand.Next(0, 2);
        currentPlayerIndex = (first + localPlayerIndex) % 2;

        for (int i = 0; i < (int)GameEnum.PlayerType.MAX; i++)
        {
            int index = (currentPlayerIndex + i) % 2;

            player[index].Init();
            player[index].SetLeader(new Leader());
            UIManager.instance.SetLeader(player[index].leader, index);
            player[index].leader.Initialize(index);
            player[index].SetPlayerID(index);
        }

        Debug.Log($"[Battle] 🥊 バトル開始 先攻: {currentPlayerIndex}");

        // ターンエンドのコールバック
        UIManager.instance.SetEndTurnButton(() => { SetCurrentState(BattleState.END_TURN); SendInputData(GameEnum.InputType.TURN_END); });  
        UIManager.instance.StartBattle();

        currentState = BattleState.START_TURN;
    }

    public void StartTurn()
    {
        // ターン開始処理
        player[currentPlayerIndex].NextTurn();
    }

    public void MainTurn()
    {
        // メインフェイズ処理

        // 相手のターンなら受信
        NetworkManager.SendBattleData data = NetworkManager.Instance.GetNextReceivedData();

        if(data.type == GameEnum.InputType.INVALID) return;

        switch (data.type)
        {
            case GameEnum.InputType.PLAY_CARD:
                // カードをプレイ
                int handIndex = data.param[0];
                UIManager.instance.PlayOpponentCard(handIndex);
                break;
            case GameEnum.InputType.ATTACK_FOLLOWER:
                // 攻撃
                int attackIndex = data.param[0];
                int defanceIndex = data.param[1];

                Debug.Log($"[Battle] 🗡️ 攻撃: {attackIndex} -> {defanceIndex}");

                CardData attackCard = field.GetOpponentFieldCard(attackIndex);
                CardData defanceCard = field.GetFieldCard(defanceIndex);

                if (attackCard == null || defanceCard == null)
                {
                    Debug.Log("[Battle] ❌ 攻撃または防御カードが存在しません");
                    return;
                }

                CardCombat(attackCard, defanceCard);
                break;

            case GameEnum.InputType.ATTACK_LEADER:
                // リーダー攻撃
                int attackLeaderIndex = data.param[0];

                Debug.Log($"[Battle] 🗡️ リーダー攻撃: {attackLeaderIndex} -> Leader");
                CardData attackLeaderCard = field.GetOpponentFieldCard(attackLeaderIndex);

                if (attackLeaderCard == null)
                {
                    Debug.Log("[Battle] ❌ 攻撃カードが存在しません");
                    return;
                }

                Leader defanceLeader = player[(currentPlayerIndex + 1) % 2].leader;
                LeaderCombat(attackLeaderCard, defanceLeader);
                break;

            case GameEnum.InputType.EVOLVE:
                // 進化
                int evolveIndex = data.param[0];
                break;

            case GameEnum.InputType.SUPER_EVOLVE:
                // 超進化
                int superEvolveIndex = data.param[0];
                break;

            case GameEnum.InputType.ACT:
                // 能力使用
                int actIndex = data.param[0];
                break;

            case GameEnum.InputType.FUSION:
                // 融合
                break;

            case GameEnum.InputType.EXTRA_PP:
            // エクストラPP
                break;

            case GameEnum.InputType.TURN_END:
                // ターン終了
                SetCurrentState(BattleState.END_TURN);
                break;
            default:
                break;
        }
    }

    public void EndTurn()
    {
        if (!UIManager.instance.IsCompleteAllSequence()) return;
        // ターン終了処理
        currentPlayerIndex = (currentPlayerIndex + 1) % 2;
    }

    public void EndBattle()
    {
        // バトル終了処理
        Debug.Log("[Battle] 🏁 バトル終了");

        // リザルト画面へ
        SceneManager.LoadScene("Result");
    }

    public void SendInputData(GameEnum.InputType type, int[] param = null)
    {
        NetworkManager.SendBattleData data = new NetworkManager.SendBattleData();
        data.type = type;
        data.param = param;
        NetworkManager.Instance.SendData(data);
    }

    public BattleState GetCurrentState() { return currentState; }

    private void SetCurrentState(BattleState setState)
    {
        currentState = setState;
    }

    public Player GetCurrentPlayer()
    {
        return player[currentPlayerIndex];
    }

    public Player GetPlayer(int index)
    {
        return player[index];
    }

    public void CardCombat(CardData attackCard, CardData defanceCard)
    {
        // 戦闘カードの登録
        CombatProcessor processor = new CombatProcessor(attackCard, defanceCard);

        processor.Combat();
    }

    public void LeaderCombat(CardData attackCard, Leader defanceLeader)
    {
        // 戦闘カードの登録
        CombatProcessor processor = new CombatProcessor(attackCard, defanceLeader);

        processor.LeaderCombat();
    }

    public bool IsOwnTurn()
    {
        return currentPlayerIndex == (int)GameEnum.PlayerType.OWN;
    }

    public void LeaderDefeated(int playerID)
    {
        Debug.Log($"[Battle] 🏳️ プレイヤー{playerID}のリーダーが敗北しました");
        currentState = BattleState.END_BATTLE;
    }
}
