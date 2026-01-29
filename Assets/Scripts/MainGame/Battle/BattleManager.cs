using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
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
    private bool isProcessingState = false;
    public bool IsGame = false;

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
            leader.SetCanEvolve(true);
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

    public async void Update()
    {
        if (isProcessingState) return;

        switch (currentState)
        {
            case BattleState.START_BATTLE:
                isProcessingState = true;
                await StartBattle();
                isProcessingState = false;
                break;
            case BattleState.START_TURN:
                isProcessingState = true;
                await StartTurn();
                currentState = BattleState.MAIN_TURN;
                isProcessingState = false;
                break;
            case BattleState.MAIN_TURN:
                // メインフェイズ処理（継続的に呼ぶ想定）
                await MainTurn();
                break;
            case BattleState.END_TURN:
                isProcessingState = true;
                await EndTurn();
                isProcessingState = false;
                break;
            case BattleState.END_BATTLE:
                isProcessingState = true;
                await EndBattle();
                isProcessingState = false;
                break;
            default:
                break;
        }
    }

    public async UniTask StartBattle()
    {
        // プレイヤー数チェック
        if (NetworkManager.Instance.GetActivePlayerCount() < 2)
            return;

        IsGame = true;
        AudioManager.instance.PlayBGM(AudioManager.BGMType.BATTLE);

        // ① シード値同期
        if (localPlayerIndex == 1)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            if (NetworkManager.Instance.SendSeedData(seed) != 0)
            {
                Debug.LogError("[Battle] ❌ シード値送信失敗");
                return;
            }
            Debug.Log($"[Battle] 🌱 シード値送信: {seed}");
        }
        else
        {
            Debug.Log("[Battle] 🌱 シード値受信待機中...");
            await WaitUntil(() => NetworkManager.Instance.seedReceived);
            seed = NetworkManager.Instance.GetReceivedSeed();
            Debug.Log($"[Battle] 🎲 シード値を取得: {seed}");
        }

        // ② デッキ送信
        var myDeck = DeckRecorder.Instance.GetCurrentDeck();
        if (myDeck == null || myDeck.Count == 0)
        {
            Debug.LogError("[Battle] ❌ デッキが空です");
            return;
        }

        NetworkManager.Instance.SendDeckData(myDeck);
        Debug.Log("[Battle] 📤 デッキ送信完了");

        // ③ 相手デッキ受信待機
        Debug.Log("[Battle] 📥 相手のデッキ受信待機中...");
        await WaitUntil(() => NetworkManager.Instance.deckReceived);

        var opponentDeck = NetworkManager.Instance.GetReceivedDeck();
        Debug.Log($"[Battle] ✅ 相手デッキ受信完了（カード数: {opponentDeck.Count}）");

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

        player[0].deck.SetDeckData(DeckRecorder.Instance.GetCurrentDeck());
        player[1].deck.SetDeckData(opponentDeck);

        player[currentPlayerIndex].deck.ShuffleDeck();
        player[(currentPlayerIndex + 1) % 2].deck.ShuffleDeck();

        Debug.Log($"[Battle] 🥊 バトル開始 先攻: {currentPlayerIndex}");

        // ターンエンドのコールバック
        UIManager.instance.SetEndTurnButton(() => { SetCurrentState(BattleState.END_TURN); SendInputData(GameEnum.InputType.TURN_END); });

        // UIの演出
        await UIManager.instance.PlayStartBattleSequence(currentPlayerIndex);

        // マリガン後のデッキ通信同期
        // 自分のデッキ情報を送信
        var deckData = player[(int)GameEnum.PlayerType.OWN].deck.GetDeckCardList();
        List<int> playerDeck = new List<int>();
        foreach (var card in deckData)
        {
            playerDeck.Add(card.id);
        }
        NetworkManager.Instance.SendDeckData(playerDeck);
        Debug.Log("[Battle] 📤 デッキ送信完了");

        // ③ 相手デッキ受信待機
        Debug.Log("[Battle] 📥 相手のデッキ受信待機中...");
        await WaitUntil(() => NetworkManager.Instance.deckReceived);

        opponentDeck = NetworkManager.Instance.GetReceivedDeck();
        Debug.Log($"[Battle] ✅ 相手デッキ受信完了（カード数: {opponentDeck.Count}）");
        player[(int)GameEnum.PlayerType.OPPONENT].deck.SetDeckData(opponentDeck);

        currentState = BattleState.START_TURN;
    }

    public async UniTask StartTurn()
    {
        // ターン開始処理
        player[currentPlayerIndex].NextTurn();
        UIManager.instance.StartTurn(IsOwnTurn());

        // 自分のターンなら手札とフィールドのカードの選択可能状態を更新
        if (!IsOwnTurn()) return;
        player[currentPlayerIndex].hand.SetOwnHandCardPlayable(true);
        player[currentPlayerIndex].hand.UpdatePlayableCards();
        field.OnStartTurn();
    }

    public async UniTask MainTurn()
    {
        // メインフェイズ処理

        // 相手のターンなら受信
        NetworkManager.SendBattleData data = NetworkManager.Instance.GetNextReceivedData();

        if (data.type == GameEnum.InputType.INVALID) return;

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
                UIManager.instance.SetDefenceFollower(defanceIndex, attackIndex);
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
                UIManager.instance.SetAttackLeaderSequence(attackLeaderCard.GetObject());
                break;

            case GameEnum.InputType.EVOLVE:
                // 進化
                int evolveIndex = data.param[0];
                CardObject evolveCard = UIManager.instance.GetOpponentCard(evolveIndex);
                evolveCard.EvolveFollower();
                break;

            case GameEnum.InputType.SUPER_EVOLVE:
                // 超進化
                int superEvolveIndex = data.param[0];
                CardObject superEvolveCard = UIManager.instance.GetOpponentCard(superEvolveIndex);
                superEvolveCard.SuperEvolveFollower();
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

    public async UniTask EndTurn()
    {
        if (!UIManager.instance.IsCompleteAllSequence()) return;
        // 自分の手札をプレイ不能にする
        if (IsOwnTurn())
        {
            player[currentPlayerIndex].hand.SetOwnHandCardPlayable(false);
            field.OnEndTurn();
        }
        // ターン終了処理
        currentPlayerIndex = (currentPlayerIndex + 1) % 2;

        currentState = BattleState.START_TURN;
    }

    public async UniTask EndBattle()
    {
        // バトル終了処理
        Debug.Log("[Battle] 🏁 バトル終了");

        // 通信切断
        NetworkManager.Instance.Disconnect();
        IsGame = false;
        AudioManager.instance.PlayBGM(AudioManager.BGMType.OUTGAME);

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

    public async UniTask WaitUntil(Func<bool> condition)
    {
        while (!condition())
            await UniTask.Delay(50);
    }

    // 相手の場に守護持ちがいるか
    public bool IsWardOpponentField()
    {
        return field.IsWardOpponentField();
    }

    /// <summary>
    /// ターゲットを受け取り、該当するコンポーネントを返す
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<BaseComponent> GetTargetCard(Target target, bool isOwn)
    {
        List<BaseComponent> result = new List<BaseComponent>();
        // TargetSideの反転
        if (!isOwn)
        {
            switch (target.targetSide)
            {
                case Target.TargetSide.Own:
                    target.targetSide = Target.TargetSide.Opponent;
                    break;
                case Target.TargetSide.Opponent:
                    target.targetSide = Target.TargetSide.Own;
                    break;
                default: break;
            }
        }
            
        // 選択が必要か
        if (target.isSelect)
        {

        }
        // 選択不要
        else
        {
            // 領域ごとに取得
            switch (target.targetZone)
            {
                case Target.TargetZone.Hand:
                    if (target.targetSide == Target.TargetSide.Own)
                    {
                        result.AddRange(player[0].hand.GetCards(target.condition));
                    }
                    else
                    {
                        result.AddRange(player[1].hand.GetCards(target.condition));
                    }
                    break;
                case Target.TargetZone.Field:
                    result.AddRange(field.GetCards(target.targetSide, target.condition));
                    break;
                case Target.TargetZone.Leader:
                    if (target.targetSide == Target.TargetSide.Own)
                    {
                        result.Add(player[0].leader);
                    }
                    else if (target.targetSide == Target.TargetSide.Opponent)
                    {
                        result.Add(player[1].leader);
                    }
                    else if (target.targetSide == Target.TargetSide.Both)
                    {
                        result.Add(player[0].leader);
                        result.Add(player[1].leader);
                    }
                    break;
                case Target.TargetZone.FieldAndLeader:
                    result.AddRange(field.GetCards(target.targetSide, target.condition));
                    if (target.targetSide == Target.TargetSide.Own)
                    {
                        result.Add(player[0].leader);
                    }
                    else if (target.targetSide == Target.TargetSide.Opponent)
                    {
                        result.Add(player[1].leader);
                    }
                    else if (target.targetSide == Target.TargetSide.Both)
                    {
                        result.Add(player[0].leader);
                        result.Add(player[1].leader);
                    }
                    break;
                default: break;
            }
            // ランダムに除外
            if (target.isRandom)
            {
                int takeCount = result.Count - target.count;
                for (int i = 0, max = takeCount; i < max; i++)
                {
                    result.RemoveAt(rand.Next(0, result.Count));
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 条件からカードを検索
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public List<CardData> GetCards(List<CardData> cardList, TargetCondition condition)
    {
        for (int i = 0, cardMax = cardList.Count; i < cardMax; i++)
        {
            // IDチェック
            if (condition.ID != null && condition.ID == cardList[i].id)
            {
                cardList.Remove(cardList[i]);
                continue;
            }
            // タイプチェック
            if (condition.type != null)
            {
                for (int j = 0, max = condition.type.Count; j < max; j++)
                {
                    if (condition.type[j] == cardList[i].type)
                    {
                        cardList.Remove(cardList[i]);
                        continue;
                    }
                }
            }
            // リーダークラスチェック
            if (condition.leaderClass != null)
            {
                for (int j = 0, max = condition.leaderClass.Count; j < max; j++)
                {
                    if (condition.leaderClass[j] != cardList[i].leaderClass)
                    {
                        cardList.Remove(cardList[i]);
                        continue;
                    }
                }
            }
            // カード詳細タイプチェック
            if (condition.cardTypeDetail != null)
            {
                for (int j = 0, max = condition.cardTypeDetail.Count; j < max; j++)
                {
                    if (!cardList[i].HaveDetailType(condition.cardTypeDetail[j]))
                    {
                        cardList.Remove(cardList[i]);
                        continue;
                    }
                }
            }
            // 進化状態チェック
            if (condition.evolveState != CardData.EvolveState.None || cardList[i].evolveState == condition.evolveState)
            {
                cardList.Remove(cardList[i]);
                continue;
            }
            // 攻撃力範囲チェック
            if (!condition.attack.Match(cardList[i].status.m_attack))
            {
                cardList.Remove(cardList[i]);
                continue;
            }
            // 体力範囲チェック
            if (!condition.defence.Match(cardList[i].status.m_defance))
            {
                cardList.Remove(cardList[i]);
                continue;
            }
            // ダメージ状態チェック
            if (condition.isHurt != null && condition.isHurt == cardList[i].damage < 1)
            {
                cardList.Remove(cardList[i]);
                continue;
            }
        }
        return cardList;
    }

    // ゲームを終了
    public void ExitGame()
    {
        Debug.Log("[Battle] ❌ 相手が切断されました。ゲームを終了します。");
        NetworkManager.Instance.Disconnect();
        IsGame = false;
        AudioManager.instance.PlayBGM(AudioManager.BGMType.OUTGAME);
        SceneManager.LoadScene("Title");
    }
}
