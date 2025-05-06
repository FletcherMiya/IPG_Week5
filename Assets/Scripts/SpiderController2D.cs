using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiderController2D : MonoBehaviour
{
    [Header("Leg Groups")]
    public List<SpiderLeg2D> leftLegs = new List<SpiderLeg2D>();
    public List<SpiderLeg2D> rightLegs = new List<SpiderLeg2D>();

    [Header("Timing")]
    public float stepInterval = 0.2f;      // 每条腿踏步后的冷却时间
    public float interSideDelay = 0.1f;    // 左右腿之间最小起步间隔

    private bool isLeftLegMoving = false;
    private bool isRightLegMoving = false;

    private float lastStepTime = -999f;    // 上一次起步时间
    private int lastSideMoved = 0;         // -1 = 左，+1 = 右

    private Vector3 lastBodyPos;
    private float idleTimer = 0f;

    [Header("Idle Repositioning")]
    public Transform bodyTransform;
    public float idleThreshold = 0.5f; // 玩家静止多少秒后触发归位模式
    private bool isIdleStepping = false;

    void Update()
    {
        if (bodyTransform == null) return;

        bool isCurrentlyStill = (bodyTransform.position - lastBodyPos).sqrMagnitude < 0.0001f;

        if (isCurrentlyStill)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0f;
            isIdleStepping = false;

            // 玩家一旦移动 → 重置所有归位标记
            foreach (var leg in leftLegs) leg.ResetIdleStep();
            foreach (var leg in rightLegs) leg.ResetIdleStep();
        }

        lastBodyPos = bodyTransform.position;

        bool forceStep = false;

        if (!isIdleStepping && idleTimer >= idleThreshold)
        {
            isIdleStepping = true;
            forceStep = true;
        }

        TryStepGroup(leftLegs, -1, forceStep);
        TryStepGroup(rightLegs, +1, forceStep);
    }

    void TryStepGroup(List<SpiderLeg2D> legs, int side, bool forceStep = false)
    {
        bool isGroupStepping = (side == -1) ? isLeftLegMoving : isRightLegMoving;
        if (isGroupStepping) return;

        if (!forceStep && Time.time - lastStepTime < interSideDelay && lastSideMoved != side)
            return;

        SpiderLeg2D bestLeg = null;
        float maxUrgency = 0f;

        foreach (var leg in legs)
        {
            if (!forceStep && !leg.IsReadyToStep())
                continue;

            if (forceStep && leg.didIdleStep)
                continue; // 已经归位过，不再重复

            float urgency = leg.GetStepUrgency();
            if (urgency > maxUrgency)
            {
                maxUrgency = urgency;
                bestLeg = leg;
            }
        }

        if (bestLeg != null)
        {
            if (side == -1) isLeftLegMoving = true;
            if (side == +1) isRightLegMoving = true;

            StartCoroutine(StepLeg(bestLeg, side, forceStep));
        }
    }

    IEnumerator StepLeg(SpiderLeg2D leg, int side, bool forceStep)
    {
        lastStepTime = Time.time;
        lastSideMoved = side;

        yield return StartCoroutine(leg.Step());
        yield return new WaitForSeconds(stepInterval);

        if (side == -1) isLeftLegMoving = false;
        if (side == +1) isRightLegMoving = false;
        leg.didIdleStep = forceStep;
    }
}
