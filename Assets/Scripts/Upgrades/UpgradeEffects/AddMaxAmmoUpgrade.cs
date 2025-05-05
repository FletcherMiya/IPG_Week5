using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Max Ammo +X")]
public class AddMaxAmmoUpgrade : UpgradeEffectBase
{
    public int amount = 10;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.AddMaxamoo(amount);
        }
    }
}