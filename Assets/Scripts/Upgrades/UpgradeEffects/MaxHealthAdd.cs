using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Max Health +1")]
public class MaxHealthUpgrade : UpgradeEffectBase
{
    public float healthIncrease;

    public override void Apply(GameObject player)
    {
        var stats = player.GetComponent<Player>().stats;
        if (stats != null)
        {
            stats.maxHealth += healthIncrease;
        }
    }
}