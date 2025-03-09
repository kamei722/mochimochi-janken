using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]
    public int itemID;
    public string itemName;
    
    [TextArea(3, 5)] // 3〜5行のテキストエリア
    public string description;
    
    [Header("見た目")]
    public Sprite itemSprite;
    
    [Header("効果")]
    public ItemEffect itemEffect;
    
    [Header("使用条件（オプション）")]
    public bool hasUsageLimit = false;
    public int maxUsageCount = 1;
    
    [HideInInspector]
    public int currentUsageCount = 0;
    
    // ゲーム開始時にリセットするメソッド
    public void ResetUsage()
    {
        currentUsageCount = 0;
    }
    
    // このアイテムがまだ使用可能かどうか
    public bool CanUse()
    {
        return !hasUsageLimit || currentUsageCount < maxUsageCount;
    }
    
    // アイテムを使用した際のカウントを更新
    public void UseItem()
    {
        if (CanUse())
        {
            currentUsageCount++;
        }
    }
}