//　てだれ レベルのためのヒューリスティックエージェント
// 神視点での相性を取得　手のスコアを計算　計算には　過去3回の履歴を考慮して補正を加える
// mikanはあと4点で使用

using System;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAgent : IAgent
{
    private int typeCount;
    private System.Random random;
    private int lastAction = -1;

    private int[,] typeAdvantageTable;

    // 過去3回の選択を保存し、団子システム(異なる手×3で勝利時×3点)の期待値を計算に入れる
    private List<int> agentPreviousChoices = new List<int>();
    private int maxPreviousChoices = 3;

    // アイテム関連
    private bool usedItem = false;

    public AdvancedAgent(int[,] typeAdvantageTable, int typeCount)
    {
        this.typeCount = typeCount;
        
        // 相性表のnullチェック
        if (typeAdvantageTable == null)
        {
            //Debug.LogError("AdvancedAgent: typeAdvantageTable is null, creating default table");
            this.typeAdvantageTable = new int[typeCount, typeCount];
            
            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < typeCount; j++)
                {
                    if (i == j)
                    {
                        this.typeAdvantageTable[i, j] = 0; // 同じタイプ同士は引き分け
                    }
                    else if (j > i) // 片方だけ設定
                    {
                        this.typeAdvantageTable[i, j] = UnityEngine.Random.Range(0, 2) * 2 - 1; // -1か1
                        this.typeAdvantageTable[j, i] = -this.typeAdvantageTable[i, j]; 
                    }
                }
            }
        }
        else
        {
            this.typeAdvantageTable = typeAdvantageTable;
        }
        
        this.random = new System.Random();
    }
    public int ChooseAction()
    {
        // まず 30% の確率で完全ランダム、70% の確率でスコア計算
        double roll = random.NextDouble();
        if (roll < 0.3)
        {
            // 完全ランダム
            lastAction = random.Next(typeCount);
        }
        else
        {
            int[] actionScores = CalculateActionScores();
            int maxScore = int.MinValue;
            List<int> bestActions = new List<int>();

            for (int i = 0; i < typeCount; i++)
            {
                if (actionScores[i] > maxScore)
                {
                    maxScore = actionScores[i];
                    bestActions.Clear();
                    bestActions.Add(i);
                }
                else if (actionScores[i] == maxScore)
                {
                    bestActions.Add(i);
                }
            }

            // 同点ならランダムに選択
            lastAction = (bestActions.Count > 0)
                ? bestActions[random.Next(bestActions.Count)]
                : random.Next(typeCount);
        }

        // 履歴更新（直近 maxPreviousChoices 件）
        agentPreviousChoices.Add(lastAction);
        if (agentPreviousChoices.Count > maxPreviousChoices)
        {
            agentPreviousChoices.RemoveAt(0);
        }

        //Debug.Log($"AdvancedAgent: Chose action {lastAction}");
        return lastAction;
    }

    public void UpdateState(int playerAction, int agentAction, int result)
    {
        //Debug.Log($"AdvancedAgent UpdateState: player={playerAction}, agent={agentAction}, result={result}");
    }

    // mikan使用条件: 「目標点 - AIスコア <= 4」になったとき
    public bool ShouldUseItem()
    {
        if (usedItem) return false;

            int aiScore = GameController.Instance.GetComputerScore();
            int winningScore = GameController.Instance.GetScoreGoal();

            if ((winningScore - aiScore) <= 4)
            {
                //Debug.Log("AdvancedAgent: Should use item - close to winning");
                return true;
            }
            
            return false;
        
    }
    //mikanしか使うアイテムはない
    public string GetItemToUse()
    {
        usedItem = true;
        //Debug.Log("AdvancedAgent: Using mikan item");
        return "mikan";
    }


    private int[] CalculateActionScores()
    {
        int[] scores = new int[typeCount];

        // 、団子システムが発動するかどうかを判定
        bool triplePotential = (agentPreviousChoices.Count == 3
                                && agentPreviousChoices[0] != agentPreviousChoices[1]
                                && agentPreviousChoices[1] != agentPreviousChoices[2]
                                && agentPreviousChoices[0] != agentPreviousChoices[2]);

        for (int agentAction = 0; agentAction < typeCount; agentAction++)
        {
            int sumScore = 0;
            for (int playerAction = 0; playerAction < typeCount; playerAction++)
            {
                int adv = typeAdvantageTable[agentAction, playerAction];

                if (adv == 1)
                {
                    // 勝ち => 基本1点 + (団子で×3なら追加2点として期待値加算)
                    // ここでは相手の出す確率(等確率=1/typeCount)を掛けて期待値換算する手もあり
                    sumScore += 1;
                    if (triplePotential)
                    {
                        sumScore += 2;
                    }
                }
            }

            scores[agentAction] = sumScore;
        }

        return scores;
    }
    
    public void SetTypeCount(int newTypeCount)
    {
        if (newTypeCount != typeCount)
        {
            //Debug.Log($"AdvancedAgent: Updating typeCount from {typeCount} to {newTypeCount}");
            
            
            typeCount = newTypeCount;
        }
    }
}