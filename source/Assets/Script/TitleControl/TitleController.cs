using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    void Start(){
        AudioManager.Instance.PlayBGM();
    }
    // はじめるボタンをクリックしたときに呼び出されるメソッド
    public void OnStartButtonClick()
    {
        // メニューシーンに遷移
        SceneManager.LoadScene("MenuScene");
    }

    // 終わるボタンをクリックしたときに呼び出されるメソッド
    public void OnExitButtonClick()
    {
        // アプリケーションを終了
        Application.Quit();
    }
}

