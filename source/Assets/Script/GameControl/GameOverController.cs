using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    // ゲームオーバーUIパネルを参照
    public GameObject gameOverPanel;

    // 最終結果を表示するテキスト
    public TextMeshProUGUI finalResultText;
    
    // リトライボタン、メニューに戻るボタン、Helpボタン
    public GameObject retryButton;
    public GameObject menuButton;
    public GameObject helpButton;

    // 他のUI要素の参照
    public GameObject playerScoreText;
    public GameObject computerScoreText;
    public GameObject roundText;
    public GameObject resultText;
    public GameObject playerMochiImage;
    public GameObject computerMochiImage;
    public GameObject playerZabutonImage;
    public GameObject computerZabutonImage;
        
    // アニメーション設定の変数    
    [Header("勝利表示用の蝶")]
    public Image[] purpleButterflyImages;
    public Image[] greenButterflyImages;
    public GameObject butterflyContainer;  // 蝶を表示するコンテナ
    
    [Header("蝶のアニメーション設定")]
    public float leftStartX = -250f;       // 左側の基準X座標
    public float rightStartX = 250f;       // 右側の基準X座標
    public float verticalRange = 180f;     // 蝶の垂直移動範囲

    void Start()
    {
        // ゲーム終了パネルを初期状態で非表示
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (retryButton != null) retryButton.SetActive(false);
        if (menuButton != null) menuButton.SetActive(false);
        if (helpButton != null) helpButton.SetActive(false);

        if (finalResultText != null)
            finalResultText.gameObject.SetActive(false);
            
        // 団子システム蝶の非表示
        if (butterflyContainer != null)
            butterflyContainer.SetActive(false);
            
        if (purpleButterflyImages != null)
        {
            foreach (var butterfly in purpleButterflyImages)
            {
                if (butterfly != null)
                    butterfly.gameObject.SetActive(false);
            }
        }
        
        if (greenButterflyImages != null)
        {
            foreach (var butterfly in greenButterflyImages)
            {
                if (butterfly != null)
                    butterfly.gameObject.SetActive(false);
            }
        }
    }

    // ゲームオーバー時にUIパネルを表示し、コルーチンを開始するメソッド
    public void ShowGameOver(string resultMessage)
    {
        if (gameOverPanel != null)
        {
            StartCoroutine(ShowFinalResultAfterDelay(resultMessage, 2.0f));
        }
        else
        {
            Debug.LogError("GameOverController: gameOverPanel が設定されていません。");
        }
    }

    // 最終結果を遅延して表示し、その後ボタン群を表示するコルーチン
    IEnumerator ShowFinalResultAfterDelay(string resultMessage, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ゲームオーバーパネルを表示
        gameOverPanel.SetActive(true);

        // ボタン群が勝手に表示されないように、明示的に非表示にする
        if (retryButton != null) retryButton.SetActive(false);
        if (menuButton != null) menuButton.SetActive(false);
        if (helpButton != null) helpButton.SetActive(false);

        // その他のUI要素を非表示にする
        if (playerScoreText != null) playerScoreText.SetActive(false);
        if (computerScoreText != null) computerScoreText.SetActive(false);
        if (roundText != null) roundText.SetActive(false);
        if (resultText != null) resultText.SetActive(false);
        if (playerMochiImage != null) playerMochiImage.SetActive(false);
        if (computerMochiImage != null) computerMochiImage.SetActive(false);
        if (playerZabutonImage != null) playerZabutonImage.SetActive(false);
        if (computerZabutonImage != null) computerZabutonImage.SetActive(false);

        // 最終結果の表示
        if (finalResultText != null)
        {
            finalResultText.text = resultMessage;
            finalResultText.gameObject.SetActive(true);
            AudioManager.Instance.PlayResultSound();
        }
        else
        {
            Debug.LogError("GameOverController: finalResultText が設定されていません。");
        }

        // 勝者を示す蝶を表示
        if (butterflyContainer != null)
        {
            butterflyContainer.SetActive(true);
            
            // 勝者に応じて蝶を表示
            if (resultMessage.Contains("むらさきのかち"))
            {
                ShowWinnerButterfly(true); // プレイヤー（紫）の勝利
            }
            else if (resultMessage.Contains("みどりのかち"))
            {
                ShowWinnerButterfly(false); // コンピュータ（緑）の勝利
            }
            // 引き分けの場合は蝶を表示しない
        }

        // 最終結果表示から2秒後にボタン群を表示する
        yield return new WaitForSeconds(2.0f);
        ShowEndGameButtons();
    }

    // 勝者の蝶を表示するメソッド
    private void ShowWinnerButterfly(bool isPlayerWinner)
    {
        // 勝者の蝶の配列を選択
        Image[] butterflies = isPlayerWinner ? purpleButterflyImages : greenButterflyImages;
        
        if (butterflies != null && butterflies.Length >= 2)
        {
            if (butterflies[0] != null)
            {
                butterflies[0].gameObject.SetActive(true);
                StartCoroutine(AnimateButterflyInfinite(
                    butterflies[0].rectTransform,
                    0f,
                    leftStartX
                ));
            }
            
            if (butterflies[1] != null)
            {
                butterflies[1].gameObject.SetActive(true);
                StartCoroutine(AnimateButterflyInfinite(
                    butterflies[1].rectTransform, 
                    0f,
                    rightStartX
                ));
            }
            
            for (int i = 2; i < butterflies.Length; i++)
            {
                if (butterflies[i] != null)
                    butterflies[i].gameObject.SetActive(false);
            }
        }
    }

    // 蝶を無限にアニメーションさせるコルーチン
    private IEnumerator AnimateButterflyInfinite(RectTransform butterflyRect, float startDelay, float baseX)
    {
        if (butterflyRect == null) yield break;
        
        // 開始遅延
        if (startDelay > 0)
            yield return new WaitForSeconds(startDelay);

        // 基本パラメータ設定
        float amplitude = UnityEngine.Random.Range(60f, 100f); // 水平移動の幅
        float verticalSpeed = UnityEngine.Random.Range(0.2f, 0.5f); // 垂直移動の速さ
        float waveFrequency = UnityEngine.Random.Range(1.5f, 2.5f); // 水平移動の頻度
        float rotationSpeed = UnityEngine.Random.Range(0.8f, 1.5f); // 回転速度
        float scaleFrequency = UnityEngine.Random.Range(0.8f, 1.2f); // 拡大縮小の頻度
        
        float currentY = UnityEngine.Random.Range(-verticalRange, verticalRange);
        
        float yDirection = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        float startTime = Time.time;
        
        while (true)
        {
            float currentTime = Time.time - startTime;
            
            // 水平方向のゆらぎ
            float horizontalOffset = Mathf.Sin(currentTime * waveFrequency) * amplitude;
            
            // Y位置の更新（上下に動かす）
            currentY += yDirection * verticalSpeed;
            
            // 画面端で方向転換
            if (currentY > verticalRange || currentY < -verticalRange)
            {
                yDirection *= -1;
                currentY = Mathf.Clamp(currentY, -verticalRange, verticalRange);
            }
            
            // 位置の更新
            butterflyRect.anchoredPosition = new Vector2(baseX + horizontalOffset, currentY);
            
            float rotation = Mathf.Sin(currentTime * rotationSpeed) * 15f;
            butterflyRect.rotation = Quaternion.Euler(0f, 0f, rotation);
            
            yield return null;
        }
    }

    // ゲーム終了後のボタン群を表示するメソッド
    void ShowEndGameButtons()
    {
        if (retryButton != null) retryButton.SetActive(true);
        if (menuButton != null) menuButton.SetActive(true);
        if (helpButton != null) helpButton.SetActive(true);
    }

    // リトライボタンがクリックされたときの処理
    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // メニューに戻るボタンがクリックされたときの処理
    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // ゲームオーバーUI要素（テキスト、ボタン、蝶）の表示/非表示を切り替えるメソッド
    public void ToggleGameOverUIElements()
    {
        bool currentlyActive = finalResultText.gameObject.activeSelf;
        
        // 状態を反転して設定
        finalResultText.gameObject.SetActive(!currentlyActive);
        retryButton.SetActive(!currentlyActive);
        menuButton.SetActive(!currentlyActive);
        
        // 蝶のコンテナも同様に切り替え
        if (butterflyContainer != null)
        {
            butterflyContainer.SetActive(!currentlyActive);
        }
    }
}
