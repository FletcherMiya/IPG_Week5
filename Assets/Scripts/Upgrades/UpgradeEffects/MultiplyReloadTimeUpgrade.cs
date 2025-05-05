using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Reload Time ��Multiplier")]
public class MultiplyReloadTimeUpgrade : UpgradeEffectBase
{
    public float multiplier = 0.9f;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.MultiplyReloadTime(multiplier);
        }
    }
}