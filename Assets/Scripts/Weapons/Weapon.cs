using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform firePoint;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float lastFireTime;

    protected GameObject projectilePrefab;
    protected float fireRate;
    protected float reloadTime;
    protected int maxAmmo;
    protected float recoil;
    protected bool isAutomatic;

    #region Getters

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;

    #endregion

    protected virtual void Awake()
    {
        if (data == null)
        {
            Debug.LogError("WeaponData is not assigned on " + gameObject.name);
            return;
        }

        projectilePrefab = data.projectilePrefab;
        fireRate = data.fireRate;
        reloadTime = data.reloadTime;
        maxAmmo = data.maxAmmo;
        recoil = data.recoil;
        isAutomatic = data.isAutomatic;

        currentAmmo = data.maxAmmo;
    }

    protected virtual void Update()
    {
        if (!CanShoot()) return;

        if ((isAutomatic && Input.GetMouseButton(0)) || (!isAutomatic && Input.GetMouseButtonDown(0)))
        {
            if (Time.time - lastFireTime >= fireRate)
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
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);

        if (transform.parent != null)
        {
            GameObject shooter = transform.parent.gameObject;
            projectile.GetComponent<Projectile>().SetShooter(shooter);

            ApplyRecoil(shooter, direction);
        }
    }

    protected void ApplyRecoil(GameObject shooter, Vector2 shotDirection)
    {
        Rigidbody2D rb = shooter.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 recoilDirection = -shotDirection.normalized;
            rb.AddForce(recoilDirection * recoil, ForceMode2D.Impulse);
        }
    }

    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"{gameObject.name} is reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
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

    //Upgrade Functions

    public void MultiplyFirerate(float multiplier)
    {
        fireRate *= multiplier;
    }

    public void AddFirerate(float amount)
    {
        fireRate += amount;
    }

    public void MultiplyReloadTime(float multiplier)
    {
        reloadTime += multiplier;
    }

    public void AddReloadTime(float amount)
    {
        reloadTime += amount;
    }

    public void AddMaxamoo(int amount)
    {
        maxAmmo += amount;
    }

    public void MultiplyRecoil(float multiplier)
    {
        recoil *= multiplier;
    }

    public void AddRecoil(float amount)
    {
        recoil += amount;
    }

    public void SetAutomatic(bool mode)
    {
        isAutomatic = mode;
    }

    public void SetProjectile(GameObject newProjectile)
    {
        projectilePrefab = newProjectile;
    }
}
