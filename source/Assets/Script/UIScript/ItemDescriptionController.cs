//アイテム複数実装時に説明をするためのパネル　　

// // ItemDescriptionController.cs
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using TMPro;

// public class ItemDescriptionController : MonoBehaviour
// {
//     public GameObject itemDescriptionPrefab; // プレハブ
//     private List<ItemData> itemDataList; // アイテムデータのリスト

//     void Start()
//     {
//         LoadItemData();
//         GenerateItemDescriptions();
//     }

//     // ScriptableObject をロードするメソッド
//     void LoadItemData()
//     {
//         // Resources/Items フォルダに配置された全ての ItemData をロード
//         ItemData[] items = Resources.LoadAll<ItemData>("Items");
//         if (items.Length == 0)
//         {
//             Debug.LogError("No ItemData found in Resources/Items folder.");
//         }
//         itemDataList = new List<ItemData>(items);
//     }

//     // アイテムの説明を生成するメソッド
//     void GenerateItemDescriptions()
//     {
//         foreach (ItemData item in itemDataList)
//         {
//             GameObject itemEntry = Instantiate(itemDescriptionPrefab, transform);

//             // アイテム画像の設定
//             Transform imageTransform = itemEntry.transform.Find("ItemImage");
//             if (imageTransform != null)
//             {
//                 Image itemImage = imageTransform.GetComponent<Image>();
//                 if (itemImage != null)
//                 {
//                     itemImage.sprite = item.itemSprite;
//                     if (itemImage.sprite == null)
//                     {
//                         Debug.LogError("アイテム画像が見つかりません: " + item.itemName);
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError("ItemImage に Image コンポーネントがありません。");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("ItemDescriptionPrefab に ItemImage オブジェクトが見つかりません。");
//             }

//             // アイテム説明の設定
//             Transform descriptionTransform = itemEntry.transform.Find("ItemDescriptionText");
//             if (descriptionTransform != null)
//             {
//                 TextMeshProUGUI descriptionText = descriptionTransform.GetComponent<TextMeshProUGUI>();
//                 if (descriptionText != null)
//                 {
//                     descriptionText.text = item.description;
//                 }
//                 else
//                 {
//                     Debug.LogError("ItemDescriptionText に TextMeshProUGUI コンポーネントがありません。");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("ItemDescriptionPrefab に ItemDescriptionText オブジェクトが見つかりません。");
//             }
//         }
//     }
// }

