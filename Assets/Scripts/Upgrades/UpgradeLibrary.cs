using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class UpgradeLibrary
{
    private static List<UpgradeData> allUpgrades;

    public static void LoadUpgrades()
    {
        if (allUpgrades == null)
        {
            allUpgrades = Resources.LoadAll<UpgradeData>("Upgrades").ToList();
        }
    }

    public static List<UpgradeData> GetRandomUpgrades(int count)
    {
        LoadUpgrades();

        List<UpgradeData> result = new List<UpgradeData>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}