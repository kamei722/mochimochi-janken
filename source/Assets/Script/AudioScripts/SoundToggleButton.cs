using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        // AudioManager.Instance が初期化されるまで待機
        while (AudioManager.Instance == null)
        {
            yield return null;
        }

        // ボタンのアイコンを更新
        UpdateIcon();
    }

    public void OnToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            bool isMuted = !AudioManager.Instance.IsMuted();
            AudioManager.Instance.SetMute(isMuted);
            UpdateIcon();
        }
        else
        {
            Debug.LogWarning("AudioManager instance is null.");
        }
    }

    void UpdateIcon()
    {
        if (buttonImage != null && AudioManager.Instance != null)
        {
            bool isMuted = AudioManager.Instance.IsMuted();
            buttonImage.sprite = isMuted ? soundOffIcon : soundOnIcon;
        }
    }
}
