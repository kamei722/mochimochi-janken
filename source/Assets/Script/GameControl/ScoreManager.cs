using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private int playerScore = 0;
    private int computerScore = 0;
    private int scoreGoal;
    
    private List<int> playerPreviousChoices = new List<int>();
    private List<int> computerPreviousChoices = new List<int>();

    public void Initialize(int goalScore)
    {
        this.scoreGoal = goalScore;
        ResetScores();
        //Debug.Log($"ScoreManager: Initialized with goal score = {goalScore}");
    }

    public void ResetScores()
    {
        playerScore = 0;
        computerScore = 0;
        playerPreviousChoices.Clear();
        computerPreviousChoices.Clear();
        // Debug.Log("ScoreManager: Scores and choice history reset");
    }

    // スコア操作
    public void IncreasePlayerScore(int amount)
    {
        playerScore += amount;
        // //Debug.Log($"ScoreManager: Player score increased by {amount} to {playerScore}");
    }

    public void IncreaseComputerScore(int amount)
    {
        computerScore += amount;
        // //Debug.Log($"ScoreManager: Computer score increased by {amount} to {computerScore}");
    }

    // 選択履歴の更新
    public void UpdatePlayerPreviousChoices(int choice)
    {
        playerPreviousChoices.Add(choice);
        if (playerPreviousChoices.Count > 3)
        {
            playerPreviousChoices.RemoveAt(0);
        }
        
        string choicesStr = string.Join(", ", playerPreviousChoices);
        // Debug.Log($"ScoreManager: Updated player choices to [{choicesStr}]");
    }

    public void UpdateComputerPreviousChoices(int choice)
    {
        computerPreviousChoices.Add(choice);
        if (computerPreviousChoices.Count > 3)
        {
            computerPreviousChoices.RemoveAt(0);
        }
        
        string choicesStr = string.Join(", ", computerPreviousChoices);
        // Debug.Log($"ScoreManager: Updated computer choices to [{choicesStr}]");
    }

    // 3回の選択が全て異なるかをチェック
    public bool AreLastThreeChoicesUnique(List<int> choices)
    {
        if (choices.Count < 3)
        {
            return false;
        }
        
        return (choices[choices.Count - 1] != choices[choices.Count - 2]) &&
               (choices[choices.Count - 1] != choices[choices.Count - 3]) &&
               (choices[choices.Count - 2] != choices[choices.Count - 3]);
    }

    // ゲーム終了条件チェック
    public bool CheckWinCondition(out string resultMessage)
    {
        resultMessage = "";
        
        if (playerScore >= scoreGoal)
        {
            resultMessage = "むらさきのかち";
            // Debug.Log("ScoreManager: Win condition met - Player wins");
            return true;
        }
        else if (computerScore >= scoreGoal)
        {
            resultMessage = "みどりのかち";
            // Debug.Log("ScoreManager: Win condition met - Computer wins");
            return true;
        }
        
        return false;
    }

    public bool CheckDrawCondition(int maxRounds, int currentRound, out string resultMessage)
    {
        resultMessage = "";
        
        if (currentRound >= maxRounds)
        {
            if (playerScore > computerScore)
            {
                resultMessage = "むらさきのかち";
                // Debug.Log("ScoreManager: Draw condition met - Player wins by score");
            }
            else if (computerScore > playerScore)
            {
                resultMessage = "みどりのかち";
                // Debug.Log("ScoreManager: Draw condition met - Computer wins by score");
            }
            else
            {
                resultMessage = "ひきわけ";
                // Debug.Log("ScoreManager: Draw condition met - Scores tied");
            }
            return true;
        }
        
        return false;
    }

    // プロパティとゲッター
    public int GetPlayerScore() => playerScore;
    public int GetComputerScore() => computerScore;
    public int GetScoreGoal() => scoreGoal;
    public List<int> GetPlayerPreviousChoices() => playerPreviousChoices;
    public List<int> GetComputerPreviousChoices() => computerPreviousChoices;
}