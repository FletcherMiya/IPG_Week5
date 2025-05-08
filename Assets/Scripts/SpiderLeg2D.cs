using UnityEngine;
using System.Collections;

public class SpiderLeg2D : MonoBehaviour
{
    [Header("Bone Transforms")]
    public Transform thigh;
    public Transform calf;
    public Transform foot;

    [Header("IK & Step Settings")]
    public float thighLength = 1f;
    public float calfLength = 1f;
    public float stepThreshold = 1f;
    public float stepHeight = 0.5f;
    public float stepDuration = 0.2f;
    public float restOffsetFactor = 1f;

    [Header("IK Control")]
    public Vector2 bendDirectionHint = Vector2.up;

    private Vector2 restOffset;
    private Vector2 footPos;
    private bool isStepping;

    [HideInInspector]
    public bool didIdleStep = false;

    private LineRenderer lineRenderer;

    void Start()
    {
        restOffset = transform.localPosition;
        footPos = (Vector2)transform.position + restOffset * restOffsetFactor;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.positionCount = 3;
    }

    void Update()
    {
        if (!isStepping)
            SolveAndApplyIK();
    }

    public bool IsReadyToStep()
    {
        float dist = Vector2.Distance((Vector2)transform.position + restOffset * restOffsetFactor, footPos);
        return !isStepping && dist > stepThreshold;
    }

    public IEnumerator Step()
    {
        isStepping = true;
        Vector2 startPos = footPos;
        Vector2 targetPos = (Vector2)transform.position + restOffset * restOffsetFactor;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / stepDuration;
            float h = Mathf.Sin(Mathf.PI * t) * stepHeight;
            footPos = Vector2.Lerp(startPos, targetPos, t) + new Vector2(0, h);
            SolveAndApplyIK();
            yield return null;
        }

        footPos = targetPos;
        SolveAndApplyIK();
        isStepping = false;
    }

    void SolveAndApplyIK()
    {
        Vector2 rootPos = transform.position;
        //Vector2 kneePos = SolveKnee_AlwaysUp(rootPos, footPos, thighLength, calfLength);
        Vector2 kneePos = SolveKnee(rootPos, footPos, thighLength, calfLength, bendDirectionHint.normalized);

        thigh.position = rootPos;
        thigh.up = (kneePos - rootPos).normalized;
        thigh.localScale = new Vector3(1, thighLength, 1);

        calf.position = kneePos;
        calf.up = (footPos - kneePos).normalized;
        calf.localScale = new Vector3(1, calfLength, 1);

        if (foot != null)
        {
            foot.position = footPos;
        }
            

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, rootPos);
            lineRenderer.SetPosition(1, kneePos);
            lineRenderer.SetPosition(2, footPos);
        }
    }

    Vector2 SolveKnee_AlwaysUp(Vector2 root, Vector2 foot, float len1, float len2)
    {
        Vector2 toFoot = foot - root;
        float dist = toFoot.magnitude;

        dist = Mathf.Clamp(dist, Mathf.Abs(len1 - len2) + 0.001f, len1 + len2 - 0.001f);
        float a = (len1 * len1 - len2 * len2 + dist * dist) / (2f * dist);
        float h = Mathf.Sqrt(Mathf.Max(0f, len1 * len1 - a * a));

        Vector2 dir = toFoot.normalized;
        Vector2 mid = root + dir * a;

        Vector2 perp = Vector2.Perpendicular(dir);
        if (Vector2.Dot(perp, Vector2.up) < 0)
            perp = -perp;

        Vector2 knee = mid + perp * h;
        return knee;
    }

    Vector2 SolveKnee(Vector2 root, Vector2 foot, float len1, float len2, Vector2 bendDir)
    {
        Vector2 toFoot = foot - root;
        float dist = toFoot.magnitude;

        dist = Mathf.Clamp(dist, Mathf.Abs(len1 - len2) + 0.001f, len1 + len2 - 0.001f);
        float a = (len1 * len1 - len2 * len2 + dist * dist) / (2f * dist);
        float h = Mathf.Sqrt(Mathf.Max(0f, len1 * len1 - a * a));

        Vector2 dir = toFoot.normalized;
        Vector2 mid = root + dir * a;

        Vector2 perp = Vector2.Perpendicular(dir);
        perp = Vector2.Dot(perp, bendDir) >= 0 ? perp : -perp;

        Vector2 knee = mid + perp * h;
        return knee;
    }

    public float GetStepUrgency()
    {
        Vector2 idealPos = (Vector2)transform.position + restOffset * restOffsetFactor;
        return Vector2.Distance(idealPos, footPos);
    }

    public void ResetIdleStep()
    {
        didIdleStep = false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vector2 rootPos = transform.position;
        Vector2 kneePos = SolveKnee_AlwaysUp(rootPos, footPos, thighLength, calfLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rootPos, kneePos);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(kneePos, footPos);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(footPos, 0.05f);
    }
}
