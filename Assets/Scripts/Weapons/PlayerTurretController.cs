using UnityEngine;

public class PlayerTurretController : MonoBehaviour
{
    private TurretWeapon turretWeapon;

    void Awake()
    {
        turretWeapon = GetComponent<TurretWeapon>();
    }

    void Update()
    {
        if (turretWeapon == null) return;
        if (!turretWeapon.IsReloading && turretWeapon.CurrentAmmo <= 0)
        {
            turretWeapon.StartCoroutine("Reload");
        }

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        turretWeapon.AimAt(mouseWorld);

        if ((turretWeapon.IsAutomatic && Input.GetMouseButton(0)) ||
            (!turretWeapon.IsAutomatic && Input.GetMouseButtonDown(0)))
        {
            if (Time.time - turretWeapon.LastFireTime >= turretWeapon.FireRate)
            {
                Vector2 direction = turretWeapon.barrelPivot.right.normalized;
                turretWeapon.ManualShoot(direction);
            }
        }
    }
}