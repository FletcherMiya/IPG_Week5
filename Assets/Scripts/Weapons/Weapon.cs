using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform firePoint;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float lastFireTime;

    #region Getters

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => data != null ? data.maxAmmo : 0;
    public bool IsReloading => isReloading;

    #endregion

    protected virtual void Awake()
    {
        if (data == null)
        {
            Debug.LogError("WeaponData is not assigned on " + gameObject.name);
            return;
        }

        currentAmmo = data.maxAmmo;
    }

    protected virtual void Update()
    {
        if (!CanShoot()) return;

        if ((data.isAutomatic && Input.GetMouseButton(0)) || (!data.isAutomatic && Input.GetMouseButtonDown(0)))
        {
            if (Time.time - lastFireTime >= data.fireRate)
            {
                Shoot();
            }
        }

        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    public virtual void Shoot()
    {
        GameObject player = transform.parent.gameObject;

        if (currentAmmo <= 0) return;
        if (player.GetComponent<Player>().checkDead()) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        Vector2 attackDirection = (mouseWorldPosition - firePoint.position).normalized;

        FireProjectile(attackDirection);
        currentAmmo--;
        lastFireTime = Time.time;

        ScreenEffect.Instance.TriggerEffect(0.2f, 0.1f, Color.white);
        CameraShake.Instance.Shake(0.05f, 0.05f);
    }

    protected virtual void FireProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(data.projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);

        if (transform.parent != null)
        {
            GameObject shooter = transform.parent.gameObject;
            projectile.GetComponent<Projectile>().SetShooter(shooter);

            ApplyRecoil(shooter, direction);
        }
    }

    private void ApplyRecoil(GameObject shooter, Vector2 shotDirection)
    {
        Rigidbody2D rb = shooter.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 recoilDirection = -shotDirection.normalized;
            rb.AddForce(recoilDirection * data.recoil, ForceMode2D.Impulse);
        }
    }

    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"{gameObject.name} is reloading...");
        yield return new WaitForSeconds(data.reloadTime);
        currentAmmo = data.maxAmmo;
        isReloading = false;
        Debug.Log($"{gameObject.name} reloaded!");
    }

    protected bool CanShoot()
    {
        if (isReloading) return false;
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return false;
        if (transform.parent.GetComponent<Player>().checkDead()) return false;
        return true;
    }
}
