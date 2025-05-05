using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Toggle Auto Fire")]
public class SetAutomaticUpgrade : UpgradeEffectBase
{
    public bool isAutomatic = true;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.SetAutomatic(isAutomatic);
        }
    }
}