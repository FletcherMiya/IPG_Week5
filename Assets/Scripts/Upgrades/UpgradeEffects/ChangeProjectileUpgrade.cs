using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Change Projectile")]
public class ChangeProjectileUpgrade : UpgradeEffectBase
{
    public GameObject newProjectile;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null && newProjectile != null)
        {
            weapon.SetProjectile(newProjectile);
        }
    }
}
