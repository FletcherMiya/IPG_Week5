using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected int maxAmmo;
    protected float fireRate;
    protected float reloadTime;
    protected float recoil;
    protected bool isAutomatic;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float lastFireTime;

    #region Getters

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;

    #endregion

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }

    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
    }

    protected virtual void Update()
    {
        if (isReloading) return;


        if ((isAutomatic && Input.GetMouseButton(0)) || (!isAutomatic && Input.GetMouseButtonDown(0)))
        {
            if (Time.time - lastFireTime >= fireRate)
            {
                Shoot();
            }
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    public virtual void Shoot()
    {
        GameObject player = transform.parent.gameObject;

        if (currentAmmo <= 0 || player.GetComponent<Player>().checkDead()) return;

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

    private void ApplyRecoil(GameObject shooter, Vector2 shotDirection)
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
}
