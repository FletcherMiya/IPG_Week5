using UnityEngine;

public class AssaultRifle : Weapon
{
    [SerializeField] private int Ammo = 30;
    [SerializeField] private float FireRate = 0.1f;
    [SerializeField] private float ReloadTime = 2f;
    [SerializeField] private float Recoil = 100f;
    [SerializeField] private bool IsAutomatic = true;

    protected override void Awake()
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