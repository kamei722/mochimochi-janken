using UnityEngine;
using System.Collections;
using System;

public class RoundManager : MonoBehaviour
{
    private int currentRound = 0;
    private int maxRounds = 30;
    private int timeLimit;
    private bool roundInProgress = false;
    private bool playerHasChosen = false;
    private Coroutine countdownCoroutine;

    // イベント
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnTimeUp;
    public event Action<int> OnTimerUpdate; // 時間更新通知（-1は無制限）

    public void Initialize(int timeLimit, int maxRounds = 20)
    {
        this.timeLimit = timeLimit;
        this.maxRounds = maxRounds;
        currentRound = 0;
        // Debug.Log($"RoundManager: Initialized with timeLimit={timeLimit}, maxRounds={maxRounds}");
    }

    public void StartRound()
    {
        currentRound++;
        playerHasChosen = false;
        roundInProgress = true;
        
        // Debug.Log($"RoundManager: Starting round {currentRound}");
        
        // タイマーの開始
        if (timeLimit == 0)
        {
            // 無制限モード
            OnTimerUpdate?.Invoke(-1);
            // Debug.Log("RoundManager: Timer mode is infinite");
        }
        else
        {
            // カウントダウン開始
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
            }
            countdownCoroutine = StartCoroutine(RoundCountdown());
            // Debug.Log($"RoundManager: Starting countdown timer for {timeLimit} seconds");
        }
        
        OnRoundStart?.Invoke();
    }

    private IEnumerator RoundCountdown()
    {
        int remainingTime = timeLimit;
        
        while (remainingTime > 0)
        {
            OnTimerUpdate?.Invoke(remainingTime);
            
            yield return new WaitForSeconds(1f);
            
            if (playerHasChosen)
            {
                // Debug.Log("RoundManager: Countdown stopped - player made choice");
                yield break;
            }
            
            remainingTime--;
        }
        
        OnTimerUpdate?.Invoke(0);
        HandleTimeUp();
    }

    private void HandleTimeUp()
    {
        if (playerHasChosen) return;
        
        // Debug.Log("RoundManager: Time is up!");
        StartCoroutine(HandleTimeUpCoroutine());
    }

    private IEnumerator HandleTimeUpCoroutine()
    {
        if (!playerHasChosen)
        {
            OnTimeUp?.Invoke();
            
            // 音を再生
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMochiSelect();
            }
            
            yield return new WaitForSeconds(0.8f);

        }
    }

    public void PlayerMadeChoice()
    {
        playerHasChosen = true;
        // Debug.Log("RoundManager: Player made a choice");
        
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
    }

    public void EndRound()
    {
        roundInProgress = false;
        // Debug.Log($"RoundManager: Round {currentRound} ended");
        OnRoundEnd?.Invoke();
    }

    // プロパティとゲッター
    public int GetCurrentRound() => currentRound;
    public int GetMaxRounds() => maxRounds;
    public bool IsRoundInProgress() => roundInProgress;
    public bool HasPlayerChosen() => playerHasChosen;
}