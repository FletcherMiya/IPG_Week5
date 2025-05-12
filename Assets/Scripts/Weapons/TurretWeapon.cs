// TurretAimingWeapon.cs
using UnityEngine;
using System.Collections;

public class TurretWeapon : Weapon
{
    [Header("Turret Aiming")]
    public Transform turretBase;        
    public Transform barrelPivot;       
    public float rotationSpeed;

    [Header("Effects")]
    public float recoilDistance = 0.2f;
    public float recoilReturnSpeed = 10f;

    private Vector3 originalLocalPos;
    private Coroutine recoilCoroutine;

    protected override void Awake()
    {
        base.Awake();
        if (barrelPivot != null)
            originalLocalPos = barrelPivot.localPosition;
    }
    protected override void Update()
    {
        AimTurretAtMouse();

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

    void AimTurretAtMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = turretBase.position.z;

        Vector3 direction = mouseWorld - turretBase.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentAngle = turretBase.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        turretBase.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public override void Shoot()
    {
        GameObject player = transform.parent.gameObject;

        if (currentAmmo <= 0) return;
        if (player.GetComponent<Player>().checkDead()) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return;

        Vector2 direction = barrelPivot.right.normalized;

        FireProjectile(direction);
        currentAmmo--;
        lastFireTime = Time.time;

        ScreenEffect.Instance.TriggerEffect(0.2f, 0.1f, Color.white);
        CameraShake.Instance.Shake(0.05f, 0.05f);        
        if (recoilCoroutine != null) StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(BarrelRecoil());
    }

    protected override void FireProjectile(Vector2 direction)
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

    private IEnumerator BarrelRecoil()
    {
        Vector3 recoilOffset = originalLocalPos - Vector3.right * recoilDistance;
        barrelPivot.localPosition = recoilOffset;

        while (Vector3.Distance(barrelPivot.localPosition, originalLocalPos) > 0.01f)
        {
            barrelPivot.localPosition = Vector3.Lerp(barrelPivot.localPosition, originalLocalPos, Time.deltaTime * recoilReturnSpeed);
            yield return null;
        }

        barrelPivot.localPosition = originalLocalPos;
    }
}
