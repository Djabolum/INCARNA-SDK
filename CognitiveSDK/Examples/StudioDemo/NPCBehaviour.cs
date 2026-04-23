using UnityEngine;
using CognitiveSDK.Runtime;

public class NPCBehaviour : MonoBehaviour
{
    public Renderer targetRenderer;
    public Transform animatedTarget;
    public ActionExecutor actionExecutor;

    private Color targetColor = new Color(0.65f, 0.68f, 0.75f);
    private string currentGate = "open";
    private float currentConfidence = 0.7f;
    private Vector3 baseScale = Vector3.one;
    private Vector3 basePosition = Vector3.zero;
    private Quaternion baseRotation = Quaternion.identity;

    void Start()
    {
        if (animatedTarget == null)
        {
            animatedTarget = transform;
        }

        if (actionExecutor == null)
        {
            actionExecutor = GetComponent<ActionExecutor>();
            if (actionExecutor == null)
            {
                actionExecutor = gameObject.AddComponent<ActionExecutor>();
            }
        }

        actionExecutor?.BindTarget(animatedTarget);
        baseScale = animatedTarget.localScale;
        basePosition = animatedTarget.localPosition;
        baseRotation = animatedTarget.localRotation;
    }

    void Update()
    {
        if (animatedTarget == null)
        {
            return;
        }

        if (actionExecutor != null)
        {
            transform.position = actionExecutor.EvaluateWorldPosition(transform.position);
            transform.rotation = actionExecutor.EvaluateWorldRotation(transform.rotation, transform.position);
        }

        float pulseSpeed = currentGate == "restricted" ? 3.2f : currentGate == "caution" ? 1.9f : 0.9f;
        float pulseAmplitude = currentGate == "restricted" ? 0.11f : currentGate == "caution" ? 0.06f : 0.03f;
        float scalePulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        Vector3 executorScale = actionExecutor != null ? actionExecutor.EvaluateScaleMultiplier() : Vector3.one;
        animatedTarget.localScale = Vector3.Lerp(animatedTarget.localScale, Vector3.Scale(baseScale * scalePulse, executorScale), Time.deltaTime * 6f);

        float shakeAmplitude = currentGate == "restricted" ? 0.025f : currentGate == "caution" ? 0.008f : 0f;
        Vector3 shake = new Vector3(
            Mathf.Sin(Time.time * 16f) * shakeAmplitude,
            Mathf.Cos(Time.time * 11f) * shakeAmplitude * 0.5f,
            0f
        );
        Vector3 executorOffset = actionExecutor != null ? actionExecutor.EvaluatePositionOffset() : Vector3.zero;
        Vector3 executorRotation = actionExecutor != null ? actionExecutor.EvaluateRotationOffset() : Vector3.zero;

        if (animatedTarget != transform)
        {
            animatedTarget.localPosition = Vector3.Lerp(animatedTarget.localPosition, basePosition + shake + executorOffset, Time.deltaTime * 8f);
            animatedTarget.localRotation = Quaternion.Slerp(
                animatedTarget.localRotation,
                baseRotation * Quaternion.Euler(executorRotation),
                Time.deltaTime * 6f
            );
        }

        if (targetRenderer != null)
        {
            targetRenderer.material.color = Color.Lerp(targetRenderer.material.color, targetColor, Time.deltaTime * 4f);

            if (targetRenderer.material.HasProperty("_EmissionColor"))
            {
                float gateFactor = currentGate == "restricted" ? 0.1f : currentGate == "caution" ? 0.35f : 0.8f;
                var emission = targetColor * (0.22f + Mathf.Clamp01(currentConfidence) * gateFactor);
                targetRenderer.material.SetColor("_EmissionColor", emission);
            }
        }
    }

    public void Apply(CognitiveResult result)
    {
        var baseColor = new Color(0.65f, 0.68f, 0.75f);
        switch (result.state)
        {
            case CognitiveState.Aggressive:
                baseColor = new Color(0.93f, 0.27f, 0.27f);
                break;
            case CognitiveState.Defensive:
                baseColor = new Color(0.25f, 0.54f, 0.93f);
                break;
            case CognitiveState.Unstable:
                baseColor = new Color(0.92f, 0.74f, 0.2f);
                break;
            case CognitiveState.Analytical:
                baseColor = new Color(0.55f, 0.45f, 0.93f);
                break;
            default:
                break;
        }

        if (result.behavior_gate == "restricted")
        {
            baseColor = Color.Lerp(baseColor, new Color(0.96f, 0.96f, 0.96f), 0.55f);
        }
        else if (result.behavior_gate == "caution")
        {
            baseColor = Color.Lerp(baseColor, new Color(0.98f, 0.78f, 0.16f), 0.22f);
        }

        targetColor = baseColor;
        currentGate = result.behavior_gate ?? "open";
        currentConfidence = result.behavior_confidence;
        actionExecutor?.Apply(result);
    }
}
