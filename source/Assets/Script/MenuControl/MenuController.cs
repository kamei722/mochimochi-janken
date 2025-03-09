using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Parameter Controller")]
    public MenuParameterController parameterController;
    public void OnBeginnerButtonClick()
    {
        SetAIDifficulty(AIDifficulty.Beginner);
        StartGame();
    }
    public void OnIntermediateButtonClick()
    {
        SetAIDifficulty(AIDifficulty.Intermediate);
        StartGame();
    }
    public void OnAdvancedButtonClick()
    {
        SetAIDifficulty(AIDifficulty.Advanced);
        StartGame();
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void SetAIDifficulty(AIDifficulty difficulty)
    {
        GameManager.Instance.SetAIDifficulty(difficulty);
    }

    /// MenuParameterController から餅数・先取点・制限時間の値を取得し、GameManager に保存後、GameSceneへ。
    private void StartGame()
    {
        int mochiCount = parameterController.GetMochiCountValue();
        int scoreGoal = parameterController.GetScoreGoalValue();
        int timeLimit = parameterController.GetTimeLimitValue();
        int mikanBonus = parameterController.GetMikanBonusValue();  // 追加

        GameManager.Instance.mochiCount = mochiCount;
        GameManager.Instance.scoreGoal = scoreGoal;
        GameManager.Instance.timeLimit = timeLimit;
        GameManager.Instance.mikanBonus = mikanBonus;  // 追加

        //Debug.Log($"[MenuController] StartGame: mochi={mochiCount}, scoreGoal={scoreGoal}, timeLimit={timeLimit}, mikanBonus={mikanBonus}");

        SceneManager.LoadScene("GameScene");
    }
}
