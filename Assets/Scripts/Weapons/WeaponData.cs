using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float fireRate = 0.2f;
    public float reloadTime = 1.5f;
    public int maxAmmo = 30;
    public float recoil = 100f;
    public bool isAutomatic = true;
}