using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // 既存のSE用AudioClipフィールド
    public AudioClip buttonClickSound;
    public AudioClip mochiSelectSound;
    public AudioClip explosionSound; // 新たに追加

    public AudioClip resultsound;
    public AudioClip aikosound;
    public AudioClip timeoversound;
    public AudioClip ticksound;
    public AudioClip bgmClip;
    private AudioSource bgmSource;

    private bool isMuted = false; 
    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移時にオブジェクトを保持
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は新規作成を破棄
            return;
        }
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.5f;
    }

    // SEを再生するメソッド
    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayMochiSelect()
    {
        PlaySound(mochiSelectSound);
    }

    public void PlayExplosionSound()
    {
        PlaySound(explosionSound);
    }

    // 汎用的なSE再生メソッド
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: 再生しようとしているAudioClipが設定されていません。");
            return;
        }

        // 一時的なオブジェクトを作成して音を再生
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.SetParent(this.transform);
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 0.7f; // 音量を70%に設定（必要に応じて調整）
        audioSource.playOnAwake = false;
        audioSource.Play();

        // 再生後にオブジェクトを破棄
        Destroy(tempGO, clip.length);
    }
     public void PlayBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
     public void SetMute(bool mute)
    {
        isMuted = mute;
        if (isMuted)
        {
            // BGMと効果音を停止
            bgmSource.Pause();
            AudioListener.volume = 0;
        }
        else
        {
            // BGMと効果音を再開
            bgmSource.UnPause();
            AudioListener.volume = 1;
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public void PlayResultSound()
    {
        PlaySound(resultsound);
    }

    public void PlayAikoSound()
    {
        PlaySound(aikosound);
    }

    public void PlaytimeoverSound()
    {
        PlaySound(timeoversound);
    }

    public void PlaytickSound()
    {
        PlaySound(ticksound);
    }

}
