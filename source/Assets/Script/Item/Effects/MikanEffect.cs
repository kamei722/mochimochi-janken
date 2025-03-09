// MikanEffect.cs として保存
using UnityEngine;

[CreateAssetMenu(fileName = "MikanEffect", menuName = "Items/Effects/MikanEffect")]
public class MikanEffect : ItemEffect
{
    public override void ApplyEffect(GameObject target)
    {
        // GameControllerを取得
        GameController gameController = target.GetComponent<GameController>();
        if (gameController != null)
        {
            gameController.SetMikanActive(true);
            //Debug.Log("MikanEffect applied: MikanActive = true");
        }
        else
        {
            // ItemManagerも試してみる (新しい構造用)
            ItemManager itemManager = target.GetComponent<ItemManager>();
            if (itemManager != null)
            {
                itemManager.SetMikanActive(true);
                //Debug.Log("MikanEffect applied through ItemManager");
            }
            else
            {
                //Debug.LogError("MikanEffect: Target has neither GameController nor ItemManager");
            }
        }
    }
}