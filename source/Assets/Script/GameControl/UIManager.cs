using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
     [Header("UI References")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text computerScoreText;
    [SerializeField] private TMP_Text roundText;
    
    [SerializeField] private Image playerZabutonImage;
    [SerializeField] private Image computerZabutonImage;
    [SerializeField] private Image playerMochiImage;
    [SerializeField] private Image computerMochiImage;
    
    [SerializeField] private Image[] playerDangoImages;
    [SerializeField] private Image[] computerDangoImages;
    [SerializeField] private Image playerbutterflyImage;
    [SerializeField] private Image computerbutterflyImage;
    
    [SerializeField] private ExplosionEffect explosionEffectPlayer;
    [SerializeField] private ExplosionEffect explosionEffectComputer;

    // UIEffectManagerから移動したフィールド
    [Header("Score Effects")]
    [SerializeField] private TMP_Text playerScoreEffectText;
    [SerializeField] private TMP_Text computerScoreEffectText;
    [SerializeField] private TMP_Text playerMikanEffectText;
    [SerializeField] private TMP_Text computerMikanEffectText;

    [Header("Mikan Item Display")]
    [SerializeField] private Image playerMikanItemImage;
    [SerializeField] private Image computerMikanItemImage;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip mikanEffectSound;

    [Header("Effect Settings")]
    [SerializeField] private float scoreHoldTime = 5f;
    [SerializeField] private float scoreFadeDuration = 0.5f;

    private List<Button> mochiButtons = new List<Button>();

    // プレイヤーがもちを選択したときのイベント
    public event Action<int> OnPlayerSelectMochi;

    public void Initialize()
    {
        if (playerMochiImage != null) 
            playerMochiImage.gameObject.SetActive(false);
        
        if (computerMochiImage != null) 
            computerMochiImage.gameObject.SetActive(false);
        
        HideButterflyImages();
        
        // Mikanアイテム画像を非表示
        if (playerMikanItemImage != null)
            playerMikanItemImage.gameObject.SetActive(false);
            
        if (computerMikanItemImage != null)
            computerMikanItemImage.gameObject.SetActive(false);
        
        if (resultText != null)
        {
            resultText.outlineWidth = 0.3f;  // アウトラインの太さ
            resultText.outlineColor = new Color(1, 1, 1, 0.8f);  // 白い色（80%不透明）
        }

        if (roundText != null)
        {
            roundText.outlineWidth = 0.2f;  // アウトラインの太さ
            roundText.outlineColor = new Color(1, 1, 1, 0.8f);  // 白い色（80%不透明）
        }
        
     //Debug.Log("UIManager: Initialized");
    }

    private void HideButterflyImages()
    {
        if (playerbutterflyImage != null)
            playerbutterflyImage.gameObject.SetActive(false);
            
        if (computerbutterflyImage != null)
            computerbutterflyImage.gameObject.SetActive(false);
    }

    // 餅ボタンの生成とレイアウト調整
    public void GenerateButtons(Sprite[] gameMochiSprites)
    {
        if (gameMochiSprites == null || gameMochiSprites.Length == 0)
        {
            //Debug.LogError("UIManager: Cannot generate buttons, no mochi sprites available.");
            return;
        }

        // 既存のボタンを削除
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        mochiButtons.Clear();

        // レイアウト調整
        AdjustGridLayoutGroup(gameMochiSprites.Length);
        
        // ボタンを生成
        for (int i = 0; i < gameMochiSprites.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            Button buttonComponent = newButton.GetComponent<Button>();
            
            int selectedType = i;
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() => {
                OnPlayerSelectMochi?.Invoke(selectedType);
            });
            
            mochiButtons.Add(buttonComponent);
            
            // 画像設定
            Image buttonImage = newButton.GetComponent<Image>();
            buttonImage.sprite = gameMochiSprites[i];
            buttonImage.preserveAspect = true;
            buttonImage.raycastTarget = true;
        }
        
        //Debug.Log($"UIManager: Generated {gameMochiSprites.Length} mochi buttons");
    }

    // レイアウト調整
    private void AdjustGridLayoutGroup(int buttonCount)
    {
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = buttonContainer.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                //Debug.LogError("UIManager: GridLayoutGroup not found on buttonContainer.");
                return;
            }
        }

        // 画面サイズに合わせて調整
        float screenWidth = Screen.width;
        float cellSize = screenWidth / (buttonCount + 1);
        cellSize = Mathf.Clamp(cellSize, 200, 350);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(30, 30);

        // ボタン数に応じた列数設定
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = buttonCount <= 4 ? buttonCount : Mathf.Min(buttonCount, 6);
        
        // 配置位置
        gridLayoutGroup.childAlignment = TextAnchor.LowerCenter;
        
        //Debug.Log($"UIManager: Adjusted GridLayoutGroup for {buttonCount} buttons");
    }

    // ボタン有効/無効化
    public void EnableMochiButtons()
    {
        foreach (Button button in mochiButtons)
        {
            button.interactable = true;
        }
    }

    public void DisableMochiButtons()
    {
        foreach (Button button in mochiButtons)
        {
            button.interactable = false;
        }
    }

    // テキスト表示更新
    public void SetRoundText(string text)
    {   
         // 可読性を高めるためにアウトラインを追加
        if (roundText != null)
        {
            roundText.text = text;
        }
    }

    public void SetResultText(string text)
    {   
        // 可読性を高めるためにアウトラインを追加
        if (resultText != null)
        {
            resultText.text = text;
        }
    }

    public void SetTimerText(int seconds)
    {
        if (resultText != null)
            resultText.text = $"<size=260>{seconds}</size>";
    }

    public void SetInfiniteTimerText()
    {
        if (resultText != null)
            resultText.text = "<size=260>∞</size>";
    }

    public void SetTimeUpText()
    {
        if (resultText != null)
            resultText.text = "じかんぎれ";
    }

    // スコア表示更新
    public void UpdateScoreText(int playerScore, int computerScore)
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();
            
        if (computerScoreText != null)
            computerScoreText.text = computerScore.ToString();
    }

    // もち画像表示
    public void ShowPlayerMochi(Sprite mochiSprite)
    {
        if (playerMochiImage != null)
        {
            playerMochiImage.sprite = mochiSprite;
            playerMochiImage.gameObject.SetActive(true);
        }
    }

    public void ShowComputerMochi(Sprite mochiSprite)
    {
        if (computerMochiImage != null)
        {
            computerMochiImage.sprite = mochiSprite;
            computerMochiImage.gameObject.SetActive(true);
        }
    }

    // 団子表示の更新
    public void UpdateDangoDisplay(List<int> playerChoices, List<int> computerChoices, Sprite[] mochiSprites, bool playerTriple, bool computerTriple)
    {
        // プレイヤーの団子表示
        for (int i = 0; i < playerDangoImages.Length; i++)
        {
            if (i < playerChoices.Count)
            {
                int choiceIndex = playerChoices[playerChoices.Count - 1 - i];
                playerDangoImages[i].sprite = mochiSprites[choiceIndex];
                playerDangoImages[i].gameObject.SetActive(true);
            }
            else
            {
                playerDangoImages[i].gameObject.SetActive(false);
            }
        }

        // コンピュータの団子表示
        for (int i = 0; i < computerDangoImages.Length; i++)
        {
            if (i < computerChoices.Count)
            {
                int index = computerChoices.Count - 1 - i;
                int choiceIndex = computerChoices[index];
                computerDangoImages[i].sprite = mochiSprites[choiceIndex];
                computerDangoImages[i].gameObject.SetActive(true);
            }
            else
            {
                computerDangoImages[i].gameObject.SetActive(false);
            }
        }

        // 蝶の表示
        if (playerbutterflyImage != null)
            playerbutterflyImage.gameObject.SetActive(playerTriple);
            
        if (computerbutterflyImage != null)
            computerbutterflyImage.gameObject.SetActive(computerTriple);
    }

    // エフェクト関連
    public void TriggerExplosion(bool isPlayer)
    {
        ExplosionEffect effectToTrigger = isPlayer ? explosionEffectPlayer : explosionEffectComputer;
        if (effectToTrigger != null)
        {
            //Debug.Log($"UIManager: Triggering explosion for {(isPlayer ? "player" : "computer")}");
            effectToTrigger.TriggerExplosion();
        }
    }

    public bool IsExplosionAnimationFinished(bool isPlayer)
    {
        ExplosionEffect effect = isPlayer ? explosionEffectPlayer : explosionEffectComputer;
        return effect != null && effect.IsAnimationFinished;
    }

   public void ShowScoreEffect(bool isPlayer, int basePoints, bool isTriple, bool hasMikanBonus, int mikanBonusValue = 1)
{
    TMP_Text scoreText = isPlayer ? playerScoreEffectText : computerScoreEffectText;


    scoreText.text = $"+{basePoints}";

    // 通常スコアの演出は、一時的表示→フェードアウト
    StartCoroutine(HoldThenFadeOut(scoreText, scoreHoldTime, scoreFadeDuration));

    // mikan効果があるとき、追加テキスト(+N)をラウンド終了まで表示し続ける
    if (hasMikanBonus)
    {
        TMP_Text mikanText = isPlayer ? playerMikanEffectText : computerMikanEffectText;
        // すでに表示中の場合もあるかもしれないので上書きする
        mikanText.text = $"+{mikanBonusValue}";
        AudioManager.Instance.PlaySound(mikanEffectSound);
    }
}

    public void ShowMikanEffect(bool isPlayer, bool isFailure, int mikanBonusValue = 1)
    {
        // mikanItemImage(プレイヤー/AI)を参照
        Image mikanImage = isPlayer ? playerMikanItemImage : computerMikanItemImage;
        if (mikanImage != null)
        {
            // アイコンを表示 (フェードインや拡大演出)
            mikanImage.gameObject.SetActive(true);
            mikanImage.transform.localScale = Vector3.one * 0.5f;
            mikanImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        
            // 音を再生
            if (AudioManager.Instance != null)
            {
                if (!isFailure && mikanEffectSound != null)
                {
                    AudioManager.Instance.PlaySound(mikanEffectSound);
                }
            }
        }
}
    public void ClearAllEffects()
    {
        ClearMikanEffect(true);
        ClearMikanItem(true);
        ClearMikanEffect(false);
        ClearMikanItem(false);
    }

    public void ClearMikanEffect(bool isPlayer)
    {
        TMP_Text mikanText = isPlayer ? playerMikanEffectText : computerMikanEffectText;
        if (mikanText != null)
        {
            mikanText.text = "";
        }
    }

    public void ClearMikanItem(bool isPlayer)
    {
        Image mikanImage = isPlayer ? playerMikanItemImage : computerMikanItemImage;
        if (mikanImage != null)
        {
            mikanImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator HoldThenFadeOut(TMP_Text textComp, float holdTime, float fadeDuration)
    {
        // 一定時間ホールド
        yield return new WaitForSeconds(holdTime);

        Color originalColor = textComp.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            textComp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textComp.text = "";
        textComp.color = originalColor;
    }
}
