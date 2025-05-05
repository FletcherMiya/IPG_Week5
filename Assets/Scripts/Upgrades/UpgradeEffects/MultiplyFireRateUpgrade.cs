using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Effects/Fire Rate ¡Á1")]
public class MultiplyFireRateUpgrade : UpgradeEffectBase
{
    public float multiplier = 0.8f;

    public override void Apply(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.MultiplyFirerate(multiplier);
        }
    }
}