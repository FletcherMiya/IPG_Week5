using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float fireRate;
    public float reloadTime;
    public int maxAmmo;
    public float recoil;
    public bool isAutomatic;
}