using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SmokeLineRenderer : MonoBehaviour
{
    [Header("Smoke Settings")]
    public Transform firePoint;
    public int maxSegments = 14;
    public float segmentSpawnInterval = 0.05f;
    public float segmentFloatSpeed = 0.1f;
    public float wobbleAmount = 0.05f;
    public float totalLifetime = 2.0f;

    private LineRenderer lr;
    private List<Vector3> smokePoints = new List<Vector3>();
    private float segmentTimer = 0f;
    private float lifeTimer = 0f;
    private bool isActive = false;

    // 每个点的偏移量（用于储存上下飘 + 抖动）
    private List<Vector3> dynamicOffsets = new List<Vector3>();

    public void Trigger()
    {
        smokePoints.Clear();
        dynamicOffsets.Clear();

        smokePoints.Add(firePoint.position);            // 第一个点 = 火力点，持续锚定
        dynamicOffsets.Add(Vector3.zero);

        lr.positionCount = 1;
        lr.SetPosition(0, firePoint.position);

        segmentTimer = 0f;
        lifeTimer = totalLifetime;
        isActive = true;
        lr.enabled = true;
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.textureMode = LineTextureMode.Stretch;
    }

    void Update()
    {
        if (!isActive) return;

        lifeTimer -= Time.deltaTime;
        segmentTimer -= Time.deltaTime;

        // 更新首段位置为 firePoint
        smokePoints[0] = firePoint.position;

        // 添加新的段（仅添加位置快照，之后不受 firePoint 影响）
        if (smokePoints.Count < maxSegments && segmentTimer <= 0f)
        {
            segmentTimer = segmentSpawnInterval;

            Vector3 newPoint = firePoint.position;
            smokePoints.Add(newPoint);
            dynamicOffsets.Add(Vector3.zero);
        }

        // 更新漂浮偏移（除首段以外）
        for (int i = 1; i < smokePoints.Count; i++)
        {
            float wobbleX = Mathf.Sin(Time.time * 2f + i * 0.5f) * wobbleAmount;
            float wobbleZ = Mathf.Cos(Time.time * 2.3f + i) * wobbleAmount;
            Vector3 offset = new Vector3(wobbleX, segmentFloatSpeed * Time.deltaTime, wobbleZ);

            dynamicOffsets[i] += offset;
        }

        // 合并偏移并更新 LineRenderer
        for (int i = 0; i < smokePoints.Count; i++)
        {
            Vector3 pos = smokePoints[i];
            if (i == 0)
                lr.SetPosition(i, pos); // 火力点直接绑定
            else
                lr.SetPosition(i, pos + dynamicOffsets[i]);
        }

        lr.positionCount = smokePoints.Count;

        // Alpha 淡出（基于整体生命周期）
        float alpha = Mathf.Clamp01(lifeTimer / totalLifetime);
        Color startColor = new Color(1, 1, 1, alpha);
        Color endColor = new Color(1, 1, 1, 0);
        lr.startColor = startColor;
        lr.endColor = endColor;

        if (lifeTimer <= 0f)
        {
            isActive = false;
            lr.enabled = false;
        }
    }
}

