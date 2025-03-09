// 相性表を公開するパネル
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpPanelController : MonoBehaviour
{
    public GameObject helpPanel;
    public TypeAdvantageTableController tableController;
    public GameObject[] gameUIElements;
    public Button toggleModeButton;

    private bool showComplete = false;

    // ヘルプボタンがクリックされたときに呼ばれる
    public void ToggleHelpPanel()
    {
        bool isActive = !helpPanel.activeSelf;
        helpPanel.SetActive(isActive);
        // ControlInteraction(isActive);
    }

    //　　＊＊ Heip panel　内に　オブジェクトを追加する際の処理 **

    // void ControlInteraction(bool helpActive)
    // {
    //     foreach (GameObject ui in gameUIElements)
    //     {
    //         ui.SetActive(!helpActive);
    //     }
    // }

    // 切替ボタンの OnClick に紐づける
    public void OnToggleModeButtonClick()
    {
        showComplete = !showComplete;

        tableController.showCompleteTable = showComplete;

        tableController.InitializeTable();

        TextMeshProUGUI buttonText = toggleModeButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = showComplete ? "部分表示" : "完全表示";
        }
    }
}
