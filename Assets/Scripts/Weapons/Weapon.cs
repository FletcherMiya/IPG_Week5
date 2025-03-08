using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    protected int maxAmmo;
    protected float fireRate;
    protected float reloadTime;
    protected bool isAutomatic;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float lastFireTime;

    protected virtual void Start()
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
        if (currentAmmo <= 0) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        Vector2 attackDirection = (mouseWorldPosition - firePoint.position).normalized;

        FireProjectile(attackDirection);
        currentAmmo--;
        lastFireTime = Time.time;

        ScreenEffect.Instance.TriggerEffect(0.2f, 0.1f, Color.white);
    }

    protected virtual void FireProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);
    }

    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"{gameObject.name} 正在装填...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"{gameObject.name} 装填完成！");
    }
}
