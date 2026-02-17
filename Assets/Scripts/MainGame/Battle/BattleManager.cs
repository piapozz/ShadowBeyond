using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CommonModule;

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

    private Vector2 handScrollPosOwn = Vector2.zero;
    private Vector2 handScrollPosOpp = Vector2.zero;
    private Vector2 deckScrollPosOwn = Vector2.zero;
    private Vector2 deckScrollPosOpp = Vector2.zero;
    private Vector2 fieldScrollPosOwn = Vector2.zero;
    private Vector2 fieldScrollPosOpp = Vector2.zero;

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
        public List<ActiveAbility> crestList;

        int turn;
        bool extraPoint;

        public void Init()
        {
            turn = 0;
            extraPoint = false;
            hand = new Hand();
            deck = new Deck();
            crestList = new List<ActiveAbility>();

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

        public void AddCrest(ActiveAbility crest)
        {
            crestList.Add(crest);
        }

        public void RemoveCrest(ActiveAbility crest)
        {
            crestList.Remove(crest);
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
        field.OnStartTurn(IsOwnTurn());
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
                CardData defanceCard = field.GetFieldCard(defanceIndex, Field.FieldType.ALL);

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
                UIManager.instance.SetAttackLeaderSequence(attackLeaderCard.GetCardObject());
                break;

            case GameEnum.InputType.EVOLVE:
                // 進化
                int evolveIndex = data.param[0];
                CardObject evolveCard = UIManager.instance.GetOpponentCard(evolveIndex);
                evolveCard.EvolveFollower();
                evolveCard.GetCardData().ability.Evolve(false);
                break;

            case GameEnum.InputType.SUPER_EVOLVE:
                // 超進化
                int superEvolveIndex = data.param[0];
                CardObject superEvolveCard = UIManager.instance.GetOpponentCard(superEvolveIndex);
                superEvolveCard.SuperEvolveFollower();
                superEvolveCard.GetCardData().ability.SuperEvolve(false);
                break;

            case GameEnum.InputType.ACT:
                // 能力使用
                int actIndex = data.param[0];
                CardObject actCard = UIManager.instance.GetOpponentCard(actIndex);
                // 先頭の要素を消す
                int[] newArray = new int[data.param.Length - 1];
                Array.Copy(data.param, 1, newArray, 0, data.param.Length - 1);
                // 選択したコンポーネントを渡し、アクトを実行
                actCard.GetCardData().ability.Engage(false, GetOpponentComponents(newArray));
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
        UIManager.instance.EndTurn();
        // 自分の手札をプレイ不能にする
        bool isOwn = IsOwnTurn();
        field.OnEndTurn(isOwn);
        if (isOwn)
            player[currentPlayerIndex].hand.SetOwnHandCardPlayable(false);
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

    public void CardCombat(CardData attackCard, CardData defenceCard)
    {
        // 戦闘カードの登録
        CombatProcessor processor = new CombatProcessor(attackCard, defenceCard);

        processor.Combat();
        attackCard.GetCardObject().SetAttackPermissionLook();
        defenceCard.GetCardObject().SetAttackPermissionLook();
    }

    public void LeaderCombat(CardData attackCard, Leader defenceLeader)
    {
        // 戦闘カードの登録
        CombatProcessor processor = new CombatProcessor(attackCard, defenceLeader);

        processor.LeaderCombat();
        attackCard.GetCardObject().SetAttackPermissionLook();
    }

    public bool IsOwnTurn()
    {
        return currentPlayerIndex == (int)GameEnum.PlayerType.OWN;
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

    public bool IsAttackable(CardData targetCard)
    {
        // 潜伏か威圧持ちなら攻撃できない
        if (targetCard.HaveKeyword(GameEnum.KeywordAbility.Ambush) ||
            targetCard.HaveKeyword(GameEnum.KeywordAbility.Intimidate))
            return false;
        // 守護を持っていなく、守護持ちフォロワーがいるなら攻撃できない
        if (!targetCard.HaveKeyword(GameEnum.KeywordAbility.Ward) && IsWardOpponentField())
            return false;
        return true;
    }

    /// <summary>
    /// ターゲットを受け取り、該当するコンポーネントを返す
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<BaseComponent> GetTargetComponent(Target target, bool isOwn)
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
        return result;
    }

    /// <summary>
    /// 条件からカードを検索
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public List<CardData> GetCards(List<CardData> cardList, TargetCondition condition)
    {
        return cardList.Where(card =>
        {
            if (condition.ID != null && condition.ID != card.id)
                return false;

            if (!IsEmpty(condition.type) && !condition.type.Contains(card.type))
                return false;

            if (!IsEmpty(condition.leaderClass) && !condition.leaderClass.Contains(card.leaderClass))
                return false;

            if (!IsEmpty(condition.cardTypeDetail) &&
                condition.cardTypeDetail.Any(detail => !card.HaveDetailType(detail)))
                return false;

            if (condition.evolveState != CardData.EvolveState.None &&
                card.evolveState != condition.evolveState)
                return false;

            if (!condition.attack.Match(card.status.m_attack))
                return false;

            if (!condition.defence.Match(card.status.m_defance))
                return false;

            if (condition.isHurt != null &&
                condition.isHurt != (card.damage < 1))
                return false;

            return true;

        }).ToList();
    }

    /// <summary>
    /// インデックスのリストからコンポーネントのリストを返す
    /// </summary>
    /// <param name="indexList"></param>
    /// <returns></returns>
    public List<BaseComponent> GetOpponentComponents(int[] indexList)
    {
        int indexCount = indexList.Length;
        List<BaseComponent> components = new List<BaseComponent>(indexCount);
        for (int i = 0; i < indexCount; i++)
        {
            int index = indexList[i];
            if (index == 0) components.Add(player[(int)GameEnum.PlayerType.OPPONENT].leader);
            else if (index == 1) components.Add(player[(int)GameEnum.PlayerType.OWN].leader);
            else if (index > 1)
            {
                int fixIndex = indexList[i] - 2;
                CardData card = field.GetFieldCard(fixIndex, Field.FieldType.REVERSE_ALL);
                components.Add(card);
            }
            else components.Add(null);
        }
        return components;
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
    public async Task NotifyDeckOutLose(int playerID)
    {
        await UIManager.instance.Message("ライブラリアウト", 3);
        await LeaderDefeated(playerID);
    }

    public async UniTask LeaderDefeated(int playerID)
    {
        await UIManager.instance.Message($"[Battle] 🏳️ プレイヤー{playerID}のリーダーが敗北しました", 3);
        currentState = BattleState.END_BATTLE;
    }

    // 手札を表示
    public void OnGUI()
    {
        if (!IsGame || player == null) return;

        const int width = 300;
        const int height = 1100;

        GUILayout.BeginArea(new Rect(10, 10, width, height), GUI.skin.box);
        GUILayout.Label("=== Battle Debug GUI ===");

        GUILayout.Space(5);
        GUILayout.Label($"State : {currentState}");
        GUILayout.Label($"Turn Player : {currentPlayerIndex}");
        GUILayout.Label($"Is Own Turn : {IsOwnTurn()}");

        GUILayout.Space(10);

        // ===== プレイヤー情報 =====
        for (int i = 0; i < player.Length; i++)
        {
            var p = player[i];
            if (p.leader == null) continue;

            // ===== 手札の中身 =====
            GUILayout.Label("Hand Cards:");
            // ===== スクロールビュー開始 =====
            Vector2 scrollPos = (i == (int)GameEnum.PlayerType.OWN)
            ? handScrollPosOwn
            : handScrollPosOpp;

            scrollPos = GUILayout.BeginScrollView(
                scrollPos,
                GUILayout.Height(100)
            );

            var cards = p.hand.GetCards((card) => { return card != null; });
            for (int j = 0; j < cards.Count; j++)
            {
                var card = cards[j];

                GUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.Label(
                    $"[{j}] ID:{card.id}  {card.name}",
                    GUILayout.Width(250)
                );

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10);

            // デッキの中身
            GUILayout.Label("Deck Cards:");
            Vector2 deckScrollPos = (i == (int)GameEnum.PlayerType.OWN)
            ? deckScrollPosOwn
            : deckScrollPosOpp;

            deckScrollPos = GUILayout.BeginScrollView(
                deckScrollPos,
                GUILayout.Height(100)
            );

            var deckCards = p.deck.GetCards((card) => { return card != null; });
            for (int j = 0; j < deckCards.Count; j++)
            {
                var card = deckCards[j];

                GUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.Label(
                    $"[{j}] ID:{card.id}  {card.name}",
                    GUILayout.Width(250)
                );

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10);

            // ===== 場の中身 =====
            GUILayout.Label("Field Cards:");
            // ===== スクロールビュー開始 =====
            Vector2 fieldscrollPos = (i == (int)GameEnum.PlayerType.OWN)
            ? fieldScrollPosOwn
            : fieldScrollPosOpp;

            fieldscrollPos = GUILayout.BeginScrollView(
                fieldscrollPos,
                GUILayout.Height(100)
            );

            List<CardData> fieldCards;
            if (i == 0)
            {
                fieldCards = field._ownFieldCardList;
            }
            else
            {
                fieldCards = field._opponentFieldCardList;
            }
            for (int j = 0; j < fieldCards.Count; j++)
            {
                var card = fieldCards[j];

                GUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.Label(
                    $"[{j}] ID:{card.id}  {card.name}",
                    GUILayout.Width(250)
                );

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10);

            // スクロール位置保存
            if (i == (int)GameEnum.PlayerType.OWN)
                handScrollPosOwn = scrollPos;
            else
                handScrollPosOpp = scrollPos;

            if (i == (int)GameEnum.PlayerType.OWN)
                deckScrollPosOwn = deckScrollPos;
            else
                deckScrollPosOpp = deckScrollPos;

            if (i == (int)GameEnum.PlayerType.OWN)
                fieldScrollPosOwn = fieldscrollPos;
            else
                fieldScrollPosOpp = fieldscrollPos;
        }

        GUI.enabled = IsOwnTurn() && currentState == BattleState.MAIN_TURN;
        if (GUILayout.Button("End Turn"))
        {
            SendInputData(GameEnum.InputType.TURN_END);
            SetCurrentState(BattleState.END_TURN);
        }
        GUI.enabled = true;

        GUILayout.EndArea();
    }
}