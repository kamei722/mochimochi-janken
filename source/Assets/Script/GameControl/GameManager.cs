using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AIDifficulty SelectedAIDifficulty { get; private set; } = AIDifficulty.Beginner;

    public int mochiCount = 5;     // デフォルト 5
    public int scoreGoal = 9;      // デフォルト 9
    public int timeLimit = 15;     // デフォルト 15
    public int mikanBonus = 1;     // デフォルト +1 追加

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetAIDifficulty(AIDifficulty difficulty)
    {
        SelectedAIDifficulty = difficulty;
    }
}