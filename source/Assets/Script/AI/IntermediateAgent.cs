// くろうと レベル　のためのヒューリスティックエージェント
// プレイヤー視点での情報からての単純スコアを計算
// mikanは2,3ラウンドに使用

using System;
using System.Collections.Generic;
using UnityEngine;

class IntermediateAgent : IAgent
{
    private int typeCount;
    private System.Random random = new System.Random();
    private int lastAction = -1;

    private int[,] knownTypeAdvantages;
    private List<int> agentPreviousChoices = new List<int>();
    private int maxPreviousChoices = 3;

    private bool usedItem = false;
    private int turnCount = 0;

    public IntermediateAgent(int typeCount)
    {
        this.typeCount = typeCount;
        knownTypeAdvantages = new int[typeCount, typeCount];
        // 全て未知状態 (0) に初期化
        for (int i = 0; i < typeCount; i++)
        {
            for (int j = 0; j < typeCount; j++)
            {
                knownTypeAdvantages[i, j] = 0;
            }
        }
    }

    public int ChooseAction()
    {
        // ターンカウントを増やす
        turnCount++;

        int[] actionScores = CalculateActionScores();
        bool isLateGame = IsLateGame();

        int maxScore = int.MinValue;
        List<int> bestActions = new List<int>();

        // 各手のスコアから最も高い手を抽出
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

        lastAction = (bestActions.Count > 0) ? bestActions[random.Next(bestActions.Count)] : random.Next(typeCount);

        // 履歴更新（直近 maxPreviousChoices 件）
        agentPreviousChoices.Add(lastAction);
        if (agentPreviousChoices.Count > maxPreviousChoices)
            agentPreviousChoices.RemoveAt(0);

        //Debug.Log($"IntermediateAgent: Calculated action scores (Late Game: {isLateGame}):");
        for (int i = 0; i < typeCount; i++)
        {
            //Debug.Log($"Action {i}: Score {actionScores[i]}");
        }
        //Debug.Log($"IntermediateAgent: Chose action {lastAction} with score {maxScore}");

        return lastAction;
    }

    public void UpdateState(int playerAction, int agentAction, int result)
    {
        // 結果：0＝あいこ、1＝プレイヤー勝利、2＝エージェント勝利
        if (result == 1)
            knownTypeAdvantages[agentAction, playerAction] = -1;
        else if (result == 2)
            knownTypeAdvantages[agentAction, playerAction] = 1;
        else
            knownTypeAdvantages[agentAction, playerAction] = 0;
    }

    // 新規実装: 2ターン目または3ターン目に一度だけ使う
    public bool ShouldUseItem()
    {
        if (!usedItem && (turnCount == 2 || turnCount == 3))
        {
            return true;
        }
        return false;
    }

    public string GetItemToUse()
    {
        usedItem = true;
        return "mikan";
    }

    private int[] CalculateActionScores()
    {
        int[] scores = new int[typeCount];

        //試合が終盤かを取得し値を変更するための bool変数
        bool isLateGame = IsLateGame();

        for (int i = 0; i < typeCount; i++)
        {
            int score = 0;

            for (int j = 0; j < typeCount; j++)
            {
                int advantage = knownTypeAdvantages[i, j];
                if (advantage == 1)
                    score += isLateGame ? 4 : 2;
                else if (advantage == -1)
                    score += isLateGame ? -2 : -1;
            }
            // 同じものを出し続けないための調整項
            if (agentPreviousChoices.Contains(i))
                score += isLateGame ? -1 : -2;
            else if (!isLateGame)
                score += 1;

            scores[i] = score;
        }

        return scores;
    }

    // 
    private bool IsLateGame()
    {
        int aiScore = GameController.Instance.GetComputerScore();
        int playerScore = GameController.Instance.GetPlayerScore();
        int winningScore = GameController.Instance.GetScoreGoal();
            
        return (aiScore >= winningScore / 2 || playerScore >= winningScore / 2);
        
    }
    
    public void SetTypeCount(int newTypeCount)
    {
        if (newTypeCount != typeCount)
        {
            //Debug.Log($"IntermediateAgent: Updating typeCount from {typeCount} to {newTypeCount}");
            
            int[,] newKnownTypeAdvantages = new int[newTypeCount, newTypeCount];
            
            for (int i = 0; i < Math.Min(typeCount, newTypeCount); i++)
            {
                for (int j = 0; j < Math.Min(typeCount, newTypeCount); j++)
                {
                    newKnownTypeAdvantages[i, j] = knownTypeAdvantages[i, j];
                }
            }
            
            typeCount = newTypeCount;
            knownTypeAdvantages = newKnownTypeAdvantages;
        }
    }
}