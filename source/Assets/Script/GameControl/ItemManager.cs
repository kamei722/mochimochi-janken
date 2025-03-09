using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform itemButtonContainer;
    [SerializeField] private GridLayoutGroup itemGridLayoutGroup;
    
    [Header("Items")]
    [SerializeField] private List<ItemData> itemDataList;
    
    private List<Button> itemButtons = new List<Button>();
    private ItemData selectedItem;
    
    // みかん効果の状態管理
    private bool mikanActive = false;
    private bool mikanActiveForAI = false;
    
    // アイテム使用イベント
    public event Action<ItemData> OnItemUsed;

    public void Initialize()
    {
        GenerateItemButtons();
        AdjustItemGridLayoutGroup();
        //Debug.Log("ItemManager: Initialized with " + itemDataList.Count + " items");
    }

    private void GenerateItemButtons()
    {
        // 既存のボタンをクリア
        foreach (Transform child in itemButtonContainer)
        {
            Destroy(child.gameObject);
        }
        itemButtons.Clear();
        
        // 新しいボタンを生成
        foreach (var itemData in itemDataList)
        {
            GameObject newButton = Instantiate(itemButtonPrefab, itemButtonContainer);
            
            // ボタンの画像を設定
            Image buttonImage = newButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = itemData.itemSprite;
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f); // 初期状態は暗め
                buttonImage.preserveAspect = true;
                buttonImage.raycastTarget = true;
            }
            
            // ボタンのクリックイベントを設定
            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                int index = itemButtons.Count; // キャプチャのための変数
                buttonComponent.onClick.AddListener(() => SelectItem(itemData, index));
                buttonComponent.interactable = true;
                itemButtons.Add(buttonComponent);
            }
            
            // アイテム名のラベルがあれば設定
            Text itemNameText = newButton.GetComponentInChildren<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = itemData.itemName;
            }
        }
        
        //Debug.Log("ItemManager: Generated " + itemButtons.Count + " item buttons");
    }

    private void SelectItem(ItemData itemData, int buttonIndex)
    {
        if (selectedItem == itemData)
        {
            // 選択を解除
            selectedItem = null;
            itemButtons[buttonIndex].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            //Debug.Log("ItemManager: Deselected item " + itemData.itemName);
        }
        else
        {
            // 他の選択中のアイテムを解除
            if (selectedItem != null)
            {
                int previousIndex = itemDataList.IndexOf(selectedItem);
                if (previousIndex >= 0 && previousIndex < itemButtons.Count)
                {
                    itemButtons[previousIndex].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }
            
            // アイテムを選択
            selectedItem = itemData;
            itemButtons[buttonIndex].GetComponent<Image>().color = Color.white;
            //Debug.Log("ItemManager: Selected item " + itemData.itemName);
        }
    }

    private void AdjustItemGridLayoutGroup()
    {
        if (itemGridLayoutGroup == null)
        {
            itemGridLayoutGroup = itemButtonContainer.GetComponent<GridLayoutGroup>();
            if (itemGridLayoutGroup == null)
            {
                //Debug.LogError("ItemManager: GridLayoutGroup component not found on item button container");
                return;
            }
        }
        
        // レイアウト設定を調整
        float cellSize = 180f;
        itemGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        itemGridLayoutGroup.spacing = new Vector2(20f, 20f);
        itemGridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        
        //Debug.Log("ItemManager: Adjusted grid layout");
    }

    // アイテムを使用するメソッド - GameControllerから呼ばれる
    public void UseSelectedItem()
    {
        if (selectedItem != null && selectedItem.itemEffect != null)
        {
            // アイテム効果を適用 - ItemEffectのApplyEffectが処理を担当
            selectedItem.itemEffect.ApplyEffect(this.gameObject);
            
            OnItemUsed?.Invoke(selectedItem);
            
            // 使用済みアイテムのボタン状態を更新
            int index = itemDataList.IndexOf(selectedItem);
            if (index >= 0 && index < itemButtons.Count)
            {
                Button itemButton = itemButtons[index];
                Image itemImage = itemButton.GetComponent<Image>();
                itemButton.interactable = false;
                itemImage.color = new Color(1f, 1f, 1f, 0.5f); // 使用済みを示す
            }
            selectedItem = null;
        }
        else
        {
            //Debug.Log("ItemManager: No item selected or already used");
        }
    }

    // AIがアイテムを使用 - GameControllerから呼ばれる
    public void UseSelectedItemForAI(string itemName)
    {
        // 現在は "mikan" のみを想定
        if (itemName.ToLower() == "mikan")
        {
            SetMikanEffectActiveForAI(true);
            //Debug.Log("ItemManager: AI used mikan item");
        }
        else
        {
            //Debug.LogWarning("ItemManager: AI tried to use unknown item: " + itemName);
        }
    }

    // アイテムをリセット - ラウンド終了時にGameControllerから呼ばれる
    public void ResetItems()
    {
        // アイテム効果をリセット
        mikanActive = false;
        mikanActiveForAI = false;
        
        // UIはリセットせず、使用済みアイテムはそのまま
        selectedItem = null;
        
        //Debug.Log("ItemManager: Item effects reset");
    }

    // みかん効果の管理メソッド
    public bool IsMikanActive() => mikanActive;
    public bool IsMikanEffectActiveForAI() => mikanActiveForAI;
    
    public void SetMikanActive(bool active)
    {
        mikanActive = active;
        //Debug.Log("ItemManager: Player mikan effect set to " + active);
    }
    
    public void SetMikanEffectActiveForAI(bool active)
    {
        mikanActiveForAI = active;
        //Debug.Log("ItemManager: AI mikan effect set to " + active);
    }
    
    // ゲッター
    public ItemData GetSelectedItem() => selectedItem;
    public List<ItemData> GetAvailableItems() => itemDataList;
}

// 既存のItemData、ItemEffectクラスはそのまま使用
// ただし、ItemEffect内のApplyEffectメソッドはGameControllerを参照しているので修正が必要

/*
[CreateAssetMenu(fileName = "MikanEffect", menuName = "Items/Effects/Mikan Effect")]
public class MikanEffect : ItemEffect
{
    public override void ApplyEffect(GameController gameController)
    {
        // みかん効果の適用
        gameController.SetMikanActive(true);
        //Debug.Log("Applied Mikan effect");
    }
}
*/