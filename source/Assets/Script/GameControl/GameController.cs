using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameController Instance { get; private set; }

    // 各マネージャーへの参照
    [SerializeField] private UIManager uiManager;
    [SerializeField] private MochiTypeManager typeManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private GameOverController gameOverController;

    // エージェント関連
    [SerializeField] private AgentManager agentManager;

    // ゲーム状態
    private bool gameEnded = false;
    private int playerChoice;
    private int computerChoice;

    private int previousSeconds = -1;
    
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        // シングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Debug.Log("GameController: Awake");
    }

    private void Start()
    {
        //Debug.Log("GameController: Start");
        
        // 必要に応じてコンポーネント取得
        GetRequiredComponents();
        
        // 各マネージャーの初期化
        InitializeManagers();
        
        // イベントの登録
        RegisterEvents();
        
        // ゲーム開始
        IsInitialized = true;
        StartGame();
    }

    private void GetRequiredComponents()
    {
        // コンポーネントを取得
        if (uiManager == null) uiManager = GetComponentInChildren<UIManager>();
        if (typeManager == null) typeManager = GetComponentInChildren<MochiTypeManager>();
        if (scoreManager == null) scoreManager = GetComponentInChildren<ScoreManager>();
        if (roundManager == null) roundManager = GetComponentInChildren<RoundManager>();
        if (itemManager == null) itemManager = GetComponentInChildren<ItemManager>();
        
        // AgentManagerはシングルトン
        agentManager = AgentManager.Instance;
        if (agentManager == null)
        {
            agentManager = GetComponent<AgentManager>();
            //Debug.LogWarning("GameController: AgentManager not found as singleton, attempting to get from this GameObject.");
        }
        
        // GameOverController
        if (gameOverController == null) gameOverController = FindObjectOfType<GameOverController>();
        
        //Debug.Log("GameController: All required components acquired");
    }

    private void InitializeManagers()
    {
        // GameManagerからの設定を使用
        int mochiCount = GameManager.Instance.mochiCount;
        int scoreGoal = GameManager.Instance.scoreGoal;
        int timeLimit = GameManager.Instance.timeLimit;
        
        //Debug.Log($"GameController: Initializing managers with mochiCount={mochiCount}, scoreGoal={scoreGoal}, timeLimit={timeLimit}");
        
        typeManager.Initialize(mochiCount);
        scoreManager.Initialize(scoreGoal);
        roundManager.Initialize(timeLimit,30);
        itemManager.Initialize();
        uiManager.Initialize();
        
        // もちボタンの生成
        uiManager.GenerateButtons(typeManager.GetGameMochiSprites());
    }

    private void RegisterEvents()
    {
        // UIイベント
        uiManager.OnPlayerSelectMochi += OnPlayerSelectMochi;
        
        // ラウンドイベント
        roundManager.OnRoundStart += OnRoundStart;
        roundManager.OnRoundEnd += OnRoundEnd;
        roundManager.OnTimeUp += OnTimeUp;
        roundManager.OnTimerUpdate += OnTimerUpdate;
        
        // アイテムイベント
        itemManager.OnItemUsed += OnItemUsed;
        
        //Debug.Log("GameController: All events registered");
    }

    private void StartGame()
    {
        uiManager.SetRoundText("もちを\nえらべ");
        roundManager.StartRound();
        //Debug.Log("GameController: Game started");
    }

    private void OnRoundStart()
    {
        uiManager.EnableMochiButtons();
        uiManager.UpdateScoreText(scoreManager.GetPlayerScore(), scoreManager.GetComputerScore());
        uiManager.SetRoundText("らうんど\n" + roundManager.GetCurrentRound());
        //Debug.Log($"GameController: Round {roundManager.GetCurrentRound()} started");
    }

    private void OnRoundEnd()
    {
        if (!gameEnded)
        {
            roundManager.StartRound();
        }
    }

    //時間切れはランダム選択
    private void OnTimeUp()
    {
        uiManager.SetTimeUpText();
        
        // タイムアップ時はランダム選択
        if (!roundManager.HasPlayerChosen())
        {
            StartCoroutine(TimeUpSequence());
        }
    }

    private IEnumerator TimeUpSequence()
    {
        // SEを再生
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaytimeoverSound();
        
        // 1秒待機
        yield return new WaitForSeconds(1.0f);
        
        // ランダム選択処理を実行
        int randomIndex = Random.Range(0, typeManager.GetGameMochiSprites().Length);
        OnPlayerSelectMochi(randomIndex);
        //Debug.Log($"GameController: Time up! Random choice made: {randomIndex}");
    }

  private void OnTimerUpdate(int seconds)
    {
        // 秒数が変化したかチェック
        bool secondsChanged = seconds != previousSeconds;
        previousSeconds = seconds;

        if (seconds == -1)
        {
            uiManager.SetInfiniteTimerText();
        }
        else
        {
            uiManager.SetTimerText(seconds);
            
            // 残り5秒以下になったら効果音を鳴らす（秒数が変わった時のみ）
            if (seconds <= 5 && seconds > 0 && secondsChanged)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaytickSound(); // カウントダウン用の効果音を再生
                }
            }
        }
    }
    private void OnPlayerSelectMochi(int mochiIndex)
    {
        if (roundManager.HasPlayerChosen()) return;
        
        //Debug.Log($"GameController: Player selected mochi: {mochiIndex}");
        roundManager.PlayerMadeChoice();
        uiManager.DisableMochiButtons();

        // プレイヤーの選択を保存
        playerChoice = mochiIndex;
        scoreManager.UpdatePlayerPreviousChoices(playerChoice);
        uiManager.ShowPlayerMochi(typeManager.GetGameMochiSprites()[playerChoice]);
        
        // プレイヤーアイテム使用
        itemManager.UseSelectedItem();
        
        // エージェントの選択を取得
        if (agentManager.currentAgent == null)
        {
            //Debug.LogError("GameController: agentManager.currentAgent is null");
            return;
        }
        
        computerChoice = agentManager.GetAgentAction();
        //Debug.Log($"GameController: Computer selected mochi: {computerChoice}");
        
        // エージェントのアイテム使用判定
        if (agentManager.ShouldUseItem())
        {
            string aiItemName = agentManager.GetItemToUse();
            if (!string.IsNullOrEmpty(aiItemName))
            {
                itemManager.UseSelectedItemForAI(aiItemName);
                //Debug.Log($"GameController: AI used item: {aiItemName}");
            }
        }
        
        // 結果処理
        StartCoroutine(ProcessResultCoroutine());
    }


    // ★ゲームの勝敗判定の流れを管理する重要な処理
    private IEnumerator ProcessResultCoroutine()
    {
        // コンピュータの選択を表示
        uiManager.ShowComputerMochi(typeManager.GetGameMochiSprites()[computerChoice]);
        scoreManager.UpdateComputerPreviousChoices(computerChoice);
        
        // 結果判定
        int result = typeManager.DetermineResult(playerChoice, computerChoice);
        //Debug.Log($"GameController: Result determined: {result} (0=Draw, 1=Player wins, 2=Computer wins)");
        
        // 相性の既知情報を更新
        typeManager.UpdateKnownTypeAdvantages(playerChoice, computerChoice);
        
        // 団子表示の更新
        bool playerTriple = scoreManager.AreLastThreeChoicesUnique(scoreManager.GetPlayerPreviousChoices());
        bool computerTriple = scoreManager.AreLastThreeChoicesUnique(scoreManager.GetComputerPreviousChoices());
        
        uiManager.UpdateDangoDisplay(
            scoreManager.GetPlayerPreviousChoices(), 
            scoreManager.GetComputerPreviousChoices(),
            typeManager.GetGameMochiSprites(),
            playerTriple,
            computerTriple
        );
        
        // もち選択時の待機時間
        yield return new WaitForSeconds(0.5f);
        
        // 結果に基づいて処理
        bool explosionTriggered = false;
        
        switch (result)
        {
            case 0: // あいこ
                yield return HandleDrawResult(playerTriple, computerTriple);
                break;
                
            case 1: // プレイヤー勝利
                yield return HandlePlayerWinResult(playerTriple, computerTriple);
                explosionTriggered = true;
                break;
                
            case 2: // コンピュータ勝利
                yield return HandleComputerWinResult(playerTriple, computerTriple);
                explosionTriggered = true;
                break;
        }
        
        // 爆発エフェクト再生を待機
        if (explosionTriggered)
        {
            bool isPlayerExplosion = (result == 2);
            yield return new WaitUntil(() => uiManager.IsExplosionAnimationFinished(isPlayerExplosion));

            //エフェクト終了時の待機時間
            yield return new WaitForSeconds(0.3f);
            //Debug.Log("GameController: Explosion animation completed");
        }
        
        itemManager.SetMikanActive(false);
        itemManager.SetMikanEffectActiveForAI(false);
        uiManager.ClearAllEffects();
        
        EndRound();
    }


    // あいこの処理
    // 待機時間とあいこでも点数が入るmikanのスコアの扱いを特別に設ける
    private IEnumerator HandleDrawResult(bool playerTriple, bool computerTriple)
    {
        uiManager.SetResultText("<size=140>あいこ</size>");
        //Debug.Log("GameController: Result is a draw");
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAikoSound();
            
        // みかん効果の適用
        bool playerMikanActive = itemManager.IsMikanActive();
        bool computerMikanActive = itemManager.IsMikanEffectActiveForAI();
        int mikanBonusValue = GameManager.Instance.mikanBonus; // 追加: みかんボーナス値を取得
        
        if (playerMikanActive)
        {
            scoreManager.IncreasePlayerScore(mikanBonusValue);
            // ShowMikanEffectに加えて、ShowScoreEffectも呼び出す
            uiManager.ShowMikanEffect(true, false, mikanBonusValue);
            uiManager.ShowScoreEffect(true, 0, false, true, mikanBonusValue);
            //Debug.Log($"GameController: Player gets +{mikanBonusValue} from Mikan in draw");
        }
        
        if (computerMikanActive)
        {
            scoreManager.IncreaseComputerScore(mikanBonusValue);
            // ShowMikanEffectに加えて、ShowScoreEffectも呼び出す
            uiManager.ShowMikanEffect(false, false, mikanBonusValue);
            uiManager.ShowScoreEffect(false, 0, false, true, mikanBonusValue);
            //Debug.Log($"GameController: Computer gets +{mikanBonusValue} from Mikan in draw");
        }
        
        uiManager.UpdateScoreText(scoreManager.GetPlayerScore(), scoreManager.GetComputerScore());
        
        yield return new WaitForSeconds(1f);
    }
    

    // player win の処理　
    private IEnumerator HandlePlayerWinResult(bool playerTriple, bool computerTriple)
    {
        uiManager.SetResultText("むらさきの\nかち");
        //Debug.Log("GameController: Player wins");
        
        // 基本点とボーナスの計算
        int basePoints = playerTriple ? 3 : 1;
        int mikanBonusValue = GameManager.Instance.mikanBonus; // ボーナス値を取得 
        int mikanBonus = itemManager.IsMikanActive() ? mikanBonusValue : 0; 
        int totalPoints = basePoints + mikanBonus;
        
        // スコア加算
        scoreManager.IncreasePlayerScore(totalPoints);
        uiManager.UpdateScoreText(scoreManager.GetPlayerScore(), scoreManager.GetComputerScore());
        
        uiManager.ShowScoreEffect(true, basePoints, playerTriple, mikanBonus > 0, mikanBonusValue);
        
        if (mikanBonus > 0)
        {
            uiManager.ShowMikanEffect(true, false, mikanBonusValue);
            //Debug.Log($"GameController: Player gets +{mikanBonusValue} Mikan bonus");
        }
        
        if (itemManager.IsMikanEffectActiveForAI())
        {
            uiManager.ShowMikanEffect(false, true, mikanBonusValue);
        }
        
        // 爆発エフェクト（コンピュータ側）
        uiManager.TriggerExplosion(false);
        
        yield break;
    }
    
    // computer win 時の処理　内容は　HandlePlayerWinResult　の逆
    private IEnumerator HandleComputerWinResult(bool playerTriple, bool computerTriple)
    {
        uiManager.SetResultText("みどりの\nかち");
        //Debug.Log("GameController: Computer wins");
        
        // 基本点とボーナスの計算
        int basePoints = computerTriple ? 3 : 1;
        int mikanBonusValue = GameManager.Instance.mikanBonus;
        int mikanBonus = itemManager.IsMikanEffectActiveForAI() ? mikanBonusValue : 0;
        int totalPoints = basePoints + mikanBonus;
        
        // スコア加算
        scoreManager.IncreaseComputerScore(totalPoints);
        uiManager.UpdateScoreText(scoreManager.GetPlayerScore(), scoreManager.GetComputerScore());
        
        // エフェクト表示
        uiManager.ShowScoreEffect(false, basePoints, computerTriple, mikanBonus > 0, mikanBonusValue);
        
        if (mikanBonus > 0)
        {
            uiManager.ShowMikanEffect(false, false, mikanBonusValue);
            //Debug.Log($"GameController: Computer gets +{mikanBonusValue} Mikan bonus");
        }
        
        if (itemManager.IsMikanActive())
        {
            uiManager.ShowMikanEffect(true, true, mikanBonusValue);
            //Debug.Log("GameController: Player Mikan use failed");
        }
        
        uiManager.TriggerExplosion(true);
        
        yield break;
    }

    private void EndRound()
    {
        CheckEndCondition();
        
        itemManager.ResetItems();
        
        roundManager.EndRound();
    }

    // ★ゲーム終了条件
    // スコアゴールにいずれかのプレイヤーが達する
    // 上限ラウンド数に達する
    private void CheckEndCondition()
    {
        string resultMessage;
        
        if (scoreManager.CheckWinCondition(out resultMessage))
        {
            EndGame(resultMessage);
            return;
        }
        
        if (scoreManager.CheckDrawCondition(roundManager.GetMaxRounds(), roundManager.GetCurrentRound(), out resultMessage))
        {
            EndGame(resultMessage);
            return;
        }
    }

    // GameOverControllerの呼び出し
    private void EndGame(string resultMessage)
    {
        gameEnded = true;
        uiManager.DisableMochiButtons();

        if (gameOverController != null)
        {
            gameOverController.ShowGameOver(resultMessage);
            //Debug.Log($"GameController: Game ended - {resultMessage}");
        }
        else
        {
            //Debug.LogError("GameController: GameOverController is not set");
        }
    }

    private void OnItemUsed(ItemData item)
    {
        if (item != null && item.itemEffect != null)
        {
            item.itemEffect.ApplyEffect(this.gameObject);
            //Debug.Log($"GameController: Applied item effect for {item.name}");
        }
    }

    public void SetMikanActive(bool active)
    {
        itemManager.SetMikanActive(active);
    }

    public void IncreasePlayerScore(int increment)
    {
        scoreManager.IncreasePlayerScore(increment);
        uiManager.UpdateScoreText(scoreManager.GetPlayerScore(), scoreManager.GetComputerScore());
    }

    // UI制御用のパブリックメソッド
    public void OnBackToMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
    
    public bool IsMikanActive() => itemManager.IsMikanActive();


     // もち関連のパブリックメソッド
    public Sprite[] GetGameMochiSprites() => typeManager.GetGameMochiSprites();
    public int GetPlayerChoice() => playerChoice;
    
    

    // 以下　AIが利用するメソッド
    public int GetPlayerScore() => scoreManager.GetPlayerScore();
    public int GetComputerScore() => scoreManager.GetComputerScore();
    public int GetScoreGoal() => scoreManager.GetScoreGoal();

    public int[,] GetTypeAdvantage() => typeManager.GetTypeAdvantage();
    public bool[,] GetKnownTypeAdvantages() => typeManager.GetKnownTypeAdvantages();
    public bool IsTypeAdvantageKnown(int type1, int type2) => typeManager.IsTypeAdvantageKnown(type1, type2);
    public int GetTypeCount() => typeManager.GetTypeCount();
    
   

}