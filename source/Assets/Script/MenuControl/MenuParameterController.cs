using UnityEngine;
using TMPro;

public class MenuParameterController : MonoBehaviour
{
    //デフォルト　タイプ5, スコアゴール9,時間制限15
    [Header("Mochi Count UI")]
    public TMP_Text mochiCountText;
    private int[] mochiCountOptions = { 4, 5, 6 };
    private int mochiCountIndex = 1;

    [Header("Score Goal UI")]
    public TMP_Text scoreGoalText;
    private int[] scoreGoalOptions = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
    private int scoreGoalIndex = 4;

    [Header("Time Limit UI")]
    public TMP_Text timeLimitText;
    private string[] timeLimitOptions = { "5", "10", "15", "20", "25", "30", "∞" };
    private int timeLimitIndex = 2;

    [Header("Mikan Bonus UI")]
    public TMP_Text mikanBonusText;
    private int[] mikanBonusOptions = { 1, 2, 3 };
    private int mikanBonusIndex = 0;

    void Start()
    {
        UpdateMochiCountText();
        UpdateScoreGoalText();
        UpdateTimeLimitText();
        UpdateMikanBonusText();
    }

    // ---- MochiCount ----
    public void OnMochiMinusClick()
    {
        mochiCountIndex--;
        if (mochiCountIndex < 0) mochiCountIndex = 0;
        UpdateMochiCountText();
    }
    public void OnMochiPlusClick()
    {
        mochiCountIndex++;
        if (mochiCountIndex >= mochiCountOptions.Length)
            mochiCountIndex = mochiCountOptions.Length - 1;
        UpdateMochiCountText();
    }
    void UpdateMochiCountText()
    {
        mochiCountText.text = mochiCountOptions[mochiCountIndex].ToString();
    }

    // ---- ScoreGoal ----
    public void OnScoreGoalMinusClick()
    {
        scoreGoalIndex--;
        if (scoreGoalIndex < 0) scoreGoalIndex = 0;
        UpdateScoreGoalText();
    }
    public void OnScoreGoalPlusClick()
    {
        scoreGoalIndex++;
        if (scoreGoalIndex >= scoreGoalOptions.Length)
            scoreGoalIndex = scoreGoalOptions.Length - 1;
        UpdateScoreGoalText();
    }
    void UpdateScoreGoalText()
    {
        scoreGoalText.text = scoreGoalOptions[scoreGoalIndex].ToString();
    }

    // ---- TimeLimit ----
    public void OnTimeLimitMinusClick()
    {
        timeLimitIndex--;
        if (timeLimitIndex < 0) timeLimitIndex = 0;
        UpdateTimeLimitText();
    }
    public void OnTimeLimitPlusClick()
    {
        timeLimitIndex++;
        if (timeLimitIndex >= timeLimitOptions.Length)
            timeLimitIndex = timeLimitOptions.Length - 1;
        UpdateTimeLimitText();
    }
    void UpdateTimeLimitText()
    {
        timeLimitText.text = timeLimitOptions[timeLimitIndex];
    }

    // ---- Get Methods for MenuController ----
    public int GetMochiCountValue()
    {
        return mochiCountOptions[mochiCountIndex];
    }
    public int GetScoreGoalValue()
    {
        return scoreGoalOptions[scoreGoalIndex];
    }

    //時間無制限の処理
    public int GetTimeLimitValue()
    {
        string selected = timeLimitOptions[timeLimitIndex];
        if (selected == "∞")
            return 0;
        return int.Parse(selected);
    }

     // ---- Mikan Bonus ----
    public void OnMikanBonusMinusClick()
    {
        mikanBonusIndex--;
        if (mikanBonusIndex < 0) mikanBonusIndex = 0;
        UpdateMikanBonusText();
    }

    public void OnMikanBonusPlusClick()
    {
        mikanBonusIndex++;
        if (mikanBonusIndex >= mikanBonusOptions.Length)
            mikanBonusIndex = mikanBonusOptions.Length - 1;
        UpdateMikanBonusText();
    }

    void UpdateMikanBonusText()
    {
        mikanBonusText.text = "+" + mikanBonusOptions[mikanBonusIndex].ToString();
    }

      // 取得用メソッド
    public int GetMikanBonusValue()
    {
        return mikanBonusOptions[mikanBonusIndex];
    }
}

