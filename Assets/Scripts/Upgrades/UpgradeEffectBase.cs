using UnityEngine;

public abstract class UpgradeEffectBase : ScriptableObject
{
    public abstract void Apply(GameObject player);
}