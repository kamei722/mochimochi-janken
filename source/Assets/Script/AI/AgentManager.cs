using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; }

    public IAgent currentAgent { get; private set; }
    
    public enum AgentType { Random, IntermediateAgent, Advanced }
    public AgentType selectedAgentType;
    
    // リファクタリング後のマネージャー参照
    private MochiTypeManager typeManager;
    
    // デバッグ用 UI
    public TMP_Text debugText;

    void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        //Debug.Log("AgentManager: Awake() - Singleton initialized");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ゲームシーンに入ったらエージェントを初期化
        if (scene.name == "GameScene")
        {
            //Debug.Log($"AgentManager: OnSceneLoaded({scene.name}) => InitializeAgentBasedOnDifficulty()");
            
            // マネージャー参照を取得
            typeManager = FindObjectOfType<MochiTypeManager>();
            if (typeManager == null)
            {
                //Debug.LogError("AgentManager: MochiTypeManager not found in scene!");
            }
            
            // 必要ならGameControllerの参照も取得
            GameController gameController = GameController.Instance;
            if (gameController == null)
            {
                //Debug.LogError("AgentManager: GameController.Instance is null!");
            }
            
            // 初期化処理を実行
            InitializeAgentBasedOnDifficulty();
        }
    }

    private void InitializeAgentBasedOnDifficulty()
    {
        AIDifficulty difficulty = GameManager.Instance.SelectedAIDifficulty;
        // メニューで設定された餅の数を typeCount として扱う
        int mochiCount = GameManager.Instance.mochiCount; // 4~6 等
        
        //Debug.Log($"AgentManager: Initializing agent. Difficulty={difficulty}, mochiCount={mochiCount}");

        switch (difficulty)
        {
            case AIDifficulty.Beginner:
                selectedAgentType = AgentType.Random;
                currentAgent = new RandomAgent(mochiCount);
                //Debug.Log("AgentManager: Initialized RandomAgent");
                break;
                
            case AIDifficulty.Intermediate:
                selectedAgentType = AgentType.IntermediateAgent;
                currentAgent = new IntermediateAgent(mochiCount);
                //Debug.Log("AgentManager: Initialized IntermediateAgent");
                break;
                
            case AIDifficulty.Advanced:
                selectedAgentType = AgentType.Advanced;
                
                // リファクタリング後: MochiTypeManagerから相性表を取得
                if (typeManager != null)
                {
                    int[,] table = typeManager.GetTypeAdvantage();
                    if (table != null)
                    {
                        //Debug.Log($"AgentManager: Got type advantage table of size {table.GetLength(0)}x{table.GetLength(1)}");
                        currentAgent = new AdvancedAgent(table, mochiCount);
                    }
                    else
                    {
                        int[,] fallbackTable = CreateDefaultTypeAdvantage(mochiCount);
                        currentAgent = new AdvancedAgent(fallbackTable, mochiCount);
                    }
                }
                break;
                        
            default:
                //Debug.LogError("AgentManager: Unknown AI difficulty.");
                break;
        }

        if (debugText != null && currentAgent != null)
        {
            debugText.text = $"Difficulty: {difficulty}\nAgent: {currentAgent.GetType().Name}\nTypeCount: {mochiCount}";
        }
    }

    private int[,] CreateDefaultTypeAdvantage(int size)
    {
        int[,] defaultTable = new int[size, size];
        
        // 同じタイプ同士は引き分け
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == j)
                {
                    defaultTable[i, j] = 0; // 引き分け
                }
                else if (i < j)
                {
                    // ランダムに相性を決定
                    defaultTable[i, j] = UnityEngine.Random.Range(0, 2) * 2 - 1; // -1か1
                    defaultTable[j, i] = -defaultTable[i, j];
                }
            }
            
        }
        
        return defaultTable;
    }

    // // ▼ もしゲーム中に mochiCount が変わる場合は、このメソッドを呼ぶ (オプション)
    // public void UpdateTypeCount(int newTypeCount)
    // {
    //     if (currentAgent == null) return;

    //     // 各エージェントクラスに SetTypeCount() 的なメソッドを作り、キャストして呼ぶ
    //     if (currentAgent is RandomAgent rand)
    //     {
    //         rand.SetTypeCount(newTypeCount);
    //     }
    //     else if (currentAgent is IntermediateAgent inter)
    //     {
    //         // 実装するとき用のプレースホルダ
    //         // inter.SetTypeCount(newTypeCount);
    //     }
    //     else if (currentAgent is AdvancedAgent advanced)
    //     {
    //         // 実装するとき用のプレースホルダ
    //         // advanced.SetTypeCount(newTypeCount);
    //     }
        
    //     //Debug.Log($"AgentManager: Updated typeCount to {newTypeCount} for {currentAgent.GetType().Name}");
    // }

    public int GetAgentAction()
    {
        if (currentAgent == null)
        {
            //Debug.LogWarning("AgentManager: currentAgent is null! Returning 0.");
            return 0;
        }
        int action = currentAgent.ChooseAction();
        //Debug.Log($"AgentManager: Agent {currentAgent.GetType().Name} chose action {action}");

        if (debugText != null)
        {
            debugText.text = $"Agent: {currentAgent.GetType().Name}\nChosen Action: {action}";
        }
        return action;
    }

    public void UpdateAgentState(int playerAction, int agentAction, int result)
    {
        if (currentAgent != null)
        {
            currentAgent.UpdateState(playerAction, agentAction, result);
            //Debug.Log($"AgentManager: Updated agent state with player={playerAction}, agent={agentAction}, result={result}");
        }
    }

    public bool ShouldUseItem()
    {
        bool shouldUse = currentAgent != null && currentAgent.ShouldUseItem();
        //Debug.Log($"AgentManager: ShouldUseItem check returned {shouldUse}");
        return shouldUse;
    }

    public string GetItemToUse()
    {
        string itemToUse = currentAgent != null ? currentAgent.GetItemToUse() : "";
        //Debug.Log($"AgentManager: GetItemToUse returned {itemToUse}");
        return itemToUse;
    }
}