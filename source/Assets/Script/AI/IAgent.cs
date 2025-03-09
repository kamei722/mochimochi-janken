using System.Collections;
using System;

public interface IAgent
{
    int ChooseAction();

    IEnumerator ChooseActionCoroutine(Action<int> callback)
    {
        int action = ChooseAction();
        callback(action);
        yield break;
    }

    void UpdateState(int playerAction, int agentAction, int result);

    bool ShouldUseItem();

    string GetItemToUse();
}

