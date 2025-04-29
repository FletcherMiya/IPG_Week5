using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This file contains all the upgrades.

[CreateAssetMenu(menuName = "Upgrades/Effects/Max Speed +1")]
public class MaxSpeedUpgrade : UpgradeEffectBase
{
    public float speedIncrease;

    public override void Apply(GameObject player)
    {
        var stats = player.GetComponent<Player>().stats;
        if (stats != null)
        {
            stats.speed += speedIncrease;
        }
    }
}