//勝敗の爆発エフェクト

using UnityEngine;
using UnityEngine.UI;

public class ExplosionEffect : MonoBehaviour
{
    private Animator animator;
    private Image explosionImage;
    private bool isAnimationFinished = false;

    public bool IsAnimationFinished
    {
        get { return isAnimationFinished; }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        explosionImage = GetComponent<Image>();
        gameObject.SetActive(false);

        // Animatorを事前に初期化
        animator.enabled = true;
        animator.keepAnimatorStateOnDisable  = true;
    }

  public void TriggerExplosion()
    {
        if (animator != null)
        {
            isAnimationFinished = false;
            gameObject.SetActive(true);
            animator.SetTrigger("TriggerExplosion");

            explosionImage.color = new Color(1, 1, 1, 1);
        }
    }

    // アニメーションイベントで呼び出される音声再生メソッド
    public void PlayExplosionSound()
    {
        AudioManager.Instance?.PlayExplosionSound();
    }

    public void OnExplosionAnimationFinished()
    {
        isAnimationFinished = true;
        gameObject.SetActive(false);
    }
}
