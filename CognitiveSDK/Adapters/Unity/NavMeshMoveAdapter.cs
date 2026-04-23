using UnityEngine;
using CognitiveSDK.Runtime;

public class NavMeshMoveAdapter : MonoBehaviour
{
    public Vector3 EvaluateWorldPosition(Vector3 currentPosition, Vector3 targetPosition, CognitiveResult result, float gateDamp)
    {
        string actionType = result?.action_type ?? "idle";
        if (actionType == "idle" || actionType == "dwell")
        {
            return currentPosition;
        }

        float baseSpeed = Mathf.Lerp(0.55f, 2.2f, Mathf.Clamp01(result?.action_speed ?? 0.3f));
        float speed = baseSpeed * Mathf.Clamp(gateDamp, 0.25f, 1f);
        return Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
    }

    public Vector3 EvaluatePositionOffset(CognitiveResult result, float destinationBias, float gateDamp)
    {
        string actionType = result?.action_type ?? "idle";
        float speed = Mathf.Clamp01(result?.action_speed ?? 0f);
        float dwellTime = Mathf.Max(0.5f, result?.dwell_time ?? 2f);
        float stride = Mathf.Sin(Time.time * (2.2f + speed * 3.5f));
        float hover = Mathf.Sin(Time.time * (1.1f + 1f / dwellTime));

        switch (actionType)
        {
            case "approach":
                return new Vector3(destinationBias * 0.03f, hover * 0.018f, 0.22f * gateDamp);
            case "withdraw":
                return new Vector3(destinationBias * 0.02f, hover * 0.012f, -0.18f * gateDamp);
            case "move":
                return new Vector3(destinationBias * 0.025f, stride * 0.018f, 0.14f * gateDamp);
            case "dwell":
                return new Vector3(destinationBias * 0.01f, hover * 0.022f, 0.03f * gateDamp);
            default:
                return new Vector3(0f, hover * 0.01f * gateDamp, 0f);
        }
    }
}
