using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This file contains all the upgrades.

[CreateAssetMenu(menuName = "Upgrades/Effects/Current Health +1")]
public class CurrentHealthUpgrade : UpgradeEffectBase
{
    public float healthRecover;

    public override void Apply(GameObject player)
    {
        var stats = player.GetComponent<Player>().stats;
        if (stats != null)
        {
            stats.health = Mathf.Min(stats.health + healthRecover, stats.maxHealth);
        }
    }
}