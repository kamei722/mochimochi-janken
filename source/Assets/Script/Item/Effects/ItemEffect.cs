// ItemEffect.cs として保存
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    // 新しいシグネチャ - GameObjectを受け取るようにする
    public abstract void ApplyEffect(GameObject target);
}