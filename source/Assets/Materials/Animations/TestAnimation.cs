using UnityEngine;
using System.Collections;

public class TestAnimation : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Explosion");
            StartCoroutine(HideAfterAnimation(animator));
        }
        else
        {
            Debug.LogError("Animator コンポーネントが見つかりません。");
        }
    }

    private IEnumerator HideAfterAnimation(Animator animator)
    {
        // アニメーションの長さを取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // アニメーションが終わるまで待つ
        yield return new WaitForSeconds(animationLength);

        // 爆発アニメーション終了後にオブジェクトを非表示にする
        gameObject.SetActive(false);
    }
}
