using UnityEngine;
using CognitiveSDK.Runtime;
using System;

public class ActionExecutor : MonoBehaviour
{
    public Transform motionTarget;
    public NavMeshMoveAdapter moveAdapter;
    public AnimatorAdapter animatorAdapter;
    public EnvironmentProbeAdapter environmentProbe;

    private CognitiveResult currentResult;
    private Transform resolvedTarget;

    public void BindTarget(Transform target)
    {
        if (motionTarget == null)
        {
            motionTarget = target;
        }

        EnsureAdapters();
    }

    public void Apply(CognitiveResult result)
    {
        EnsureAdapters();
        currentResult = result;
        if (motionTarget != null && environmentProbe != null)
        {
            resolvedTarget = environmentProbe.ResolveTargetTransform(currentResult, motionTarget.position);
        }
    }

    public Vector3 EvaluatePositionOffset()
    {
        if (moveAdapter == null || environmentProbe == null) return Vector3.zero;
        return moveAdapter.EvaluatePositionOffset(
            currentResult,
            environmentProbe.GetDestinationBias(currentResult),
            environmentProbe.GetGateDamp(currentResult)
        );
    }

    public Vector3 EvaluateScaleMultiplier()
    {
        if (animatorAdapter == null) return Vector3.one;
        return animatorAdapter.EvaluateScaleMultiplier(currentResult);
    }

    public Vector3 EvaluateRotationOffset()
    {
        if (animatorAdapter == null || environmentProbe == null) return Vector3.zero;
        return animatorAdapter.EvaluateRotationOffset(
            currentResult,
            environmentProbe.GetDestinationBias(currentResult)
        );
    }

    public Vector3 EvaluateWorldPosition(Vector3 currentPosition)
    {
        if (moveAdapter == null || environmentProbe == null || resolvedTarget == null)
        {
            return currentPosition;
        }

        return moveAdapter.EvaluateWorldPosition(
            currentPosition,
            resolvedTarget.position,
            currentResult,
            environmentProbe.GetGateDamp(currentResult)
        );
    }

    public Quaternion EvaluateWorldRotation(Quaternion currentRotation, Vector3 currentPosition)
    {
        if (resolvedTarget == null)
        {
            return currentRotation;
        }

        Vector3 look = resolvedTarget.position - currentPosition;
        look.y = 0f;
        if (look.sqrMagnitude < 0.001f)
        {
            return currentRotation;
        }

        return Quaternion.Slerp(
            currentRotation,
            Quaternion.LookRotation(look.normalized, Vector3.up),
            Time.deltaTime * 3.2f
        );
    }

    public WorldStateDto BuildWorldState(Vector3 origin)
    {
        EnsureAdapters();
        return environmentProbe != null ? environmentProbe.BuildWorldState(origin) : null;
    }

    public AffordanceDto[] BuildAffordances(Vector3 origin)
    {
        EnsureAdapters();
        return environmentProbe != null ? environmentProbe.BuildAffordances(origin) : Array.Empty<AffordanceDto>();
    }

    public string DescribeCurrentTarget()
    {
        return resolvedTarget != null ? resolvedTarget.name : "hold_position";
    }

    private void EnsureAdapters()
    {
        if (moveAdapter == null)
        {
            moveAdapter = GetComponent<NavMeshMoveAdapter>();
            if (moveAdapter == null)
            {
                moveAdapter = gameObject.AddComponent<NavMeshMoveAdapter>();
            }
        }

        if (animatorAdapter == null)
        {
            animatorAdapter = GetComponent<AnimatorAdapter>();
            if (animatorAdapter == null)
            {
                animatorAdapter = gameObject.AddComponent<AnimatorAdapter>();
            }
        }

        if (environmentProbe == null)
        {
            environmentProbe = GetComponent<EnvironmentProbeAdapter>();
            if (environmentProbe == null)
            {
                environmentProbe = gameObject.AddComponent<EnvironmentProbeAdapter>();
            }
        }
    }
}
