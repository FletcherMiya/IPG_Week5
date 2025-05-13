using UnityEngine;

public class AITurretController : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;
    public Transform aimTarget;
    public Transform moveTarget;
    public float idleScanInterval;
    public float idleScanRadius;
    public float idleScanMinAngle;
    public float idleScanMaxAngle;

    [Header("FOV")]
    public float visionRadius = 10f;
    [Range(0f, 180f)]
    public float visionAngle;
    public float closeRangeOverride;

    [Header("Aiming")]
    public float aimLerpSpeed = 5f;
    public float hitChance = 0.7f;
    public float missAngleOffset = 5f;

    [Header("Moving")]
    public float moveRadius = 3.0f;
    public float minMoveRadius = 1.0f;
    public float reselectThreshold = 3.0f;

    private TurretWeapon turretWeapon;
    private float idleScanTimer = 0f;
    private Vector3 idleAimPosition;
    private bool isInIdleScan = false;

    private bool isAlertedByDamage = false;
    private float alertDuration = 5f;
    private float alertTimer = 0f;

    //private float aimUpdateCooldown = 0.5f;
    //private float moveUpdateCooldown = 1.0f;
    //private float aimTimer = 0f;
    //private float moveTimer = 0f;

    void Awake()
    {
        turretWeapon = GetComponentInChildren<TurretWeapon>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

        moveTarget.SetParent(null);
    }

    void Update()
    {
        if (target == null || turretWeapon == null) return;

        if (!turretWeapon.IsReloading && turretWeapon.CurrentAmmo <= 0)
        {
            turretWeapon.StartCoroutine("Reload");
            return;
        }

        if (isAlertedByDamage)
        {
            alertTimer -= Time.deltaTime;
            if (alertTimer <= 0f)
            {
                isAlertedByDamage = false;
            }
        }

        UpdateAimTarget();
        if (IsTargetVisible() && PlayerMoveTargetDistanceCheck())
        {
            UpdateMoveTarget();
        }


        if (IsTargetVisible() || IsTargetVeryClose() || isAlertedByDamage)
        {
            turretWeapon.AimAt(target.position);
            if (IsTurretAimedAtPosition(aimTarget.position))
            {
                TryShootWithAccuracy();
            }
        }

        if (!IsTargetVisible() && !isAlertedByDamage)
        {
            idleScanTimer += Time.deltaTime;

            if (idleScanTimer >= idleScanInterval)
            {
                SelectIdleAimPosition();
                idleScanTimer = 0f;
                isInIdleScan = true;
            }

            if (isInIdleScan)
            {
                turretWeapon.AimAt(idleAimPosition);

                if (IsTurretAimedAtPosition(idleAimPosition))
                {
                    isInIdleScan = false;
                }
            }
        }
        else
        {
            isInIdleScan = false;
            idleScanTimer = 0f;
        }
    }

    void UpdateAimTarget()
    {
        Vector3 desiredPosition = target.position;
        aimTarget.position = Vector3.Lerp(aimTarget.position, desiredPosition, Time.deltaTime * aimLerpSpeed);
    }

    void UpdateMoveTarget()
    {
        Vector3 offset;
        Vector3 candidate;

        int safetyCounter = 0;

        do
        {
            offset = Random.insideUnitCircle * moveRadius;
            candidate = target.position + new Vector3(offset.x, offset.y, 0f);
            safetyCounter++;
        }
        while (Vector3.Distance(candidate, target.position) < minMoveRadius && safetyCounter < 10);

        moveTarget.position = candidate;
    }

    bool PlayerMoveTargetDistanceCheck()
    {
        float playerToMoveTarget = Vector3.Distance(target.position, moveTarget.position);
        return playerToMoveTarget > reselectThreshold;
    }

    public void TryShootWithAccuracy()
    {
        if (Time.time - turretWeapon.LastFireTime < turretWeapon.FireRate) return;

        if (Random.value <= hitChance)
        {
            TryShoot(); 
        }
        else
        {
            TryShootOffset(missAngleOffset);
        }
    }

    void TryShoot()
    {
        //if (Time.time - turretWeapon.LastFireTime < turretWeapon.FireRate) return;

        Vector2 direction = turretWeapon.barrelPivot.right.normalized;
        turretWeapon.ManualShoot(direction);
    }

    void TryShootOffset(float offsetAngle)
    {
        //if (Time.time - turretWeapon.LastFireTime < turretWeapon.FireRate) return;

        Vector2 direction = turretWeapon.barrelPivot.right.normalized;

        float angle = (Random.value < 0.5f ? -1f : 1f) * offsetAngle;
        float rad = angle * Mathf.Deg2Rad;

        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        Vector2 offsetDirection = new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        ).normalized;

        turretWeapon.ManualShoot(offsetDirection);
    }

    bool IsTargetVisible()
    {
        Vector3 toTarget = target.position - turretWeapon.turretBase.position;
        float distance = toTarget.magnitude;
        float angle = Vector3.Angle(turretWeapon.turretBase.right, toTarget);

        return distance <= visionRadius && angle <= visionAngle * 0.5f;
    }

    bool IsTargetVeryClose()
    {
        float distance = Vector3.Distance(turretWeapon.turretBase.position, target.position);
        return distance <= closeRangeOverride;
    }

    bool IsTurretAimedAtPosition(Vector3 pos, float angleThreshold = 5f)
    {
        Vector3 dirToPos = (pos - turretWeapon.turretBase.position).normalized;
        Vector3 turretForward = turretWeapon.turretBase.right;

        float angle = Vector3.Angle(turretForward, dirToPos);
        return angle <= angleThreshold;
    }

    void SelectIdleAimPosition()
    {
        Vector3 forward = turretWeapon.turretBase.right;
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        float angleOffset = Random.Range(idleScanMinAngle, idleScanMaxAngle);
        angleOffset *= Random.value < 0.5f ? -1f : 1f;

        float finalAngle = baseAngle + angleOffset;

        Vector3 offset = Quaternion.Euler(0, 0, finalAngle) * Vector3.right * idleScanRadius;
        idleAimPosition = turretWeapon.turretBase.position + offset;
    }
    public void AlertFromDamage()
    {
        isAlertedByDamage = true;
        alertTimer = alertDuration;
    }
}
