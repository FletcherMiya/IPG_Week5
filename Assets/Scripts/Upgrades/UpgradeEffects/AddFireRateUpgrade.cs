using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Fire Rate +1")]
public class AddFireRateUpgrade : UpgradeEffectBase
{
    public float amount = -0.1f;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.AddFirerate(amount);
        }
    }
}
