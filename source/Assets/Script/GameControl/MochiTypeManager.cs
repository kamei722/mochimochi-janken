using UnityEngine;
using System.Collections.Generic;

public class MochiTypeManager : MonoBehaviour
{
    private int[,] typeAdvantage;
    private bool[,] knownTypeAdvantages;  // プレイヤーが知っている相性情報
    private Sprite[] mochiSprites;
    private Sprite[] gameMochiSprites;
    private List<int> unknownTypeAdvantages = new List<int>();
    private int typeCount;

    public void Initialize(int typeCount)
    {
        this.typeCount = typeCount;
        LoadMochiSprites();
        this.typeCount = Mathf.Min(typeCount, mochiSprites.Length);
        SelectRandomMochi();
        GenerateTypeAdvantages();
        InitializeKnownTypeAdvantages();
        
        //Debug.Log($"MochiTypeManager: Initialized with {this.typeCount} mochi types");
    }

    // もちスプライトをロード
    private void LoadMochiSprites()
    {
        mochiSprites = Resources.LoadAll<Sprite>("images/mochi");
        if (mochiSprites == null || mochiSprites.Length == 0)
        {
            //Debug.LogError("MochiTypeManager: No mochi sprites found in Resources/mochi folder.");
        }
    }

    // ゲームで使用するもちをランダムに選択
    private void SelectRandomMochi()
    {
        int mochiCount = Mathf.Min(typeCount, mochiSprites.Length);
        gameMochiSprites = new Sprite[mochiCount];
        List<int> usedIndices = new List<int>();

        for (int i = 0; i < mochiCount; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, mochiSprites.Length);
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);
            gameMochiSprites[i] = mochiSprites[randomIndex];
        }
        
        //Debug.Log($"MochiTypeManager: Selected {mochiCount} random mochi types");
    }

    // タイプ相性をランダムに生成
    // 相性は全勝の手が無くなるまで修正を重ねる形式
    void GenerateTypeAdvantages()
    {
        typeAdvantage = new int[typeCount, typeCount];
        
        // 同じタイプ同士は引き分け
        for (int i = 0; i < typeCount; i++)
        {
            typeAdvantage[i, i] = 0;
        }
        
        // ランダムに相性を設定
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = i + 1; j < typeCount; j++)
            {
                // ランダムに勝ち(1)か負け(-1)を設定
                typeAdvantage[i, j] = Random.Range(0, 2) == 0 ? 1 : -1;
                // 対称的に設定
                typeAdvantage[j, i] = -typeAdvantage[i, j];
            }
        }
        
        bool hasAllWinningType;
        do {
            hasAllWinningType = false;
            
            for (int i = 0; i < typeCount; i++)
            {
                int wins = 0;
                
                for (int j = 0; j < typeCount; j++)
                {
                    if (i != j && typeAdvantage[i, j] > 0)
                    {
                        wins++;
                    }
                }
                
                // 全勝なら1つだけ関係を逆転
                if (wins == typeCount - 1)
                {
                    hasAllWinningType = true;
                    int opponent;
                    do {
                        opponent = Random.Range(0, typeCount);
                    } while (opponent == i);
                    
                    typeAdvantage[i, opponent] = -1;
                    typeAdvantage[opponent, i] = 1;
                }
            }
        } while (hasAllWinningType); // 全勝の手がなくなるまで繰り返す
    }

    // 既知の相性情報を初期化
    private void InitializeKnownTypeAdvantages()
    {
        knownTypeAdvantages = new bool[typeCount, typeCount];
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < typeCount; j++)
            {
                knownTypeAdvantages[i, j] = (i == j);
            }
        }

        unknownTypeAdvantages.Clear();
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < typeCount; j++)
            {
                if (i != j && !knownTypeAdvantages[i, j])
                {
                    unknownTypeAdvantages.Add(j);
                }
            }
        }
        
        //Debug.Log("MochiTypeManager: Initialized known type advantages");
    }

    // 勝敗を判定
    public int DetermineResult(int player, int computer)
    {
        // 結果： 0 = あいこ, 1 = プレイヤー勝ち, 2 = コンピュータ勝ち
        if (player == computer)
            return 0;
        
        if (typeAdvantage[player, computer] == 1)
            return 1;
        
        if (typeAdvantage[player, computer] == -1)
            return 2;
        
        //Debug.LogWarning($"MochiTypeManager: Undefined typeAdvantage[{player}, {computer}] = {typeAdvantage[player, computer]}");
        return 0;
    }

    // 相性の既知/未知状態を更新
    public void UpdateKnownTypeAdvantages(int playerType, int computerType)
    {
        //Debug.Log($"MochiTypeManager: UpdateKnownTypeAdvantages called with ({playerType}, {computerType})");
        knownTypeAdvantages[playerType, computerType] = true;
        knownTypeAdvantages[computerType, playerType] = true;

        // 未知リストも更新
        unknownTypeAdvantages.Clear();
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < typeCount; j++)
            {
                if (i != j && !knownTypeAdvantages[i, j])
                {
                    unknownTypeAdvantages.Add(j);
                }
            }
        }
    }

    // デバッグ用：相性表を表示
    public void DebugPrintTypeAdvantages()
    {
        string matrix = "Type Advantage Matrix:\n";
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < typeCount; j++)
            {
                matrix += $"{typeAdvantage[i, j],3} ";
            }
            matrix += "\n";
        }
        //Debug.Log(matrix);
    }

    // 公開メソッド群
    public int[,] GetTypeAdvantage() => typeAdvantage;
    public bool[,] GetKnownTypeAdvantages() => knownTypeAdvantages;
    public Sprite[] GetGameMochiSprites() => gameMochiSprites;
    public bool IsTypeAdvantageKnown(int type1, int type2) => knownTypeAdvantages[type1, type2];
    public int GetTypeCount() => typeCount;
    public List<int> GetUnknownTypeAdvantages() => unknownTypeAdvantages;
}