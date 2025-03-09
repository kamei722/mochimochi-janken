using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public enum SoundType { NormalClick, MochiSelect }
    public SoundType soundType;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
        else
        {
            Debug.LogWarning("ButtonSound: Buttonコンポーネントが見つかりません。");
        }
    }

    private void PlaySound()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("ButtonSound: AudioManagerのインスタンスが存在しません。");
            return;
        }

        switch (soundType)
        {
            case SoundType.NormalClick:
                AudioManager.Instance.PlayButtonClick();
                break;
            case SoundType.MochiSelect:
                AudioManager.Instance.PlayMochiSelect();
                break;
            default:
                break;
        }
    }
}
