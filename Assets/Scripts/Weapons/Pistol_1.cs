using UnityEngine;

public class Pistol : Weapon
{
    [SerializeField] private int Ammo = 12;
    [SerializeField] private float FireRate = 0.3f;
    [SerializeField] private float ReloadTime = 1.5f;
    [SerializeField] private float Recoil = 1f;
    [SerializeField] private bool IsAutomatic = false;

    protected override void Start()
    {
        base.Start();
        maxAmmo = Ammo;
        fireRate = FireRate;
        reloadTime = ReloadTime;
        isAutomatic = IsAutomatic;
        recoil = Recoil;

        currentAmmo = maxAmmo;
    }
}