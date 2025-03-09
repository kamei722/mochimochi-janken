//相性表UIを設計するクラス

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeAdvantageTableController : MonoBehaviour
{
    public GameObject cellPrefab;
    public Sprite[] typeSprites;
    private int[,] typeAdvantage;
    private bool[,] knownTypeAdvantages;
    private int typeCount;
    public bool showCompleteTable;

    void OnEnable()
    {
        StartCoroutine(InitializeTableCoroutine());
    }

    public void InitializeTable()
    {
        StartCoroutine(InitializeTableCoroutine());
    }

    IEnumerator InitializeTableCoroutine()
    {
        // GameController が初期化されるまで待機
        GameController gameController = null;
        while (gameController == null || !gameController.IsInitialized)
        {
            gameController = FindObjectOfType<GameController>();
            yield return null;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // データ取得：GameController から相性表、既知情報、タイプ画像を取得
        typeAdvantage = gameController.GetTypeAdvantage();
        knownTypeAdvantages = gameController.GetKnownTypeAdvantages();
        typeSprites = gameController.GetGameMochiSprites();
        typeCount = typeSprites.Length;

        GenerateTypeAdvantageTable();
    }
// 黒の座布団cellにo,×,-を当てはめる
    void GenerateTypeAdvantageTable()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = typeCount + 1;

        // 相性表を生成
        for (int i = -1; i < typeCount; i++)
        {
            for (int j = -1; j < typeCount; j++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);
                Image cellImage = cell.GetComponent<Image>();
                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();

                if (cellImage == null || cellText == null)
                {
                    //Debug.LogError("Cell prefab に Image または TextMeshProUGUI コンポーネントが設定されていません。");
                    continue;
                }

                if (i == -1 && j == -1)
                {
                    // 左上隅のセル（空白）
                    cellText.text = "";
                    cellImage.color = Color.clear;
                }
                else if (i == -1)
                {
                    // 列の見出し（プレイヤーのタイプ）
                    cellImage.sprite = typeSprites[j];
                    cellText.text = "";
                }
                else if (j == -1)
                {
                    // 行の見出し（相手のタイプ）
                    cellImage.sprite = typeSprites[i];
                    cellText.text = "";
                }
                else
                {
                    int advantage = typeAdvantage[j, i];
                    if (advantage == 1)
                    {
                        cellText.text = "O";
                    }
                    else if (advantage == -1)
                    {
                        cellText.text = "×";
                    }
                    else
                    {
                        cellText.text = "-";
                    }
                }
            }
        }
    }

}
