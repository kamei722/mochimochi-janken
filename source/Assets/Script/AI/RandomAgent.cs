// しろうと　レベルのためのエージェント　
// ランダムに手を出力
using UnityEngine;

public class RandomAgent : IAgent
{
    private int typeCount;

    public RandomAgent(int typeCount)
    {
        this.typeCount = typeCount;
    }

    public void SetTypeCount(int newCount)
    {
        this.typeCount = newCount;
    }

    public string GetItemToUse()
    {
        return ""; // ランダムAIはアイテムを使わない
    }

    public int ChooseAction()
    {
        // もし typeCount <= 0 ならエラー防止
        if (typeCount <= 0)
        {
            //Debug.LogWarning("RandomAgent: typeCount <= 0, returning 0");
            return 0;
        }
        return Random.Range(0, typeCount);
    }

    public void UpdateState(int playerAction, int agentAction, int result)
    {
        // 何もしない
    }

    public bool ShouldUseItem()
    {
        return false;
    }
}
