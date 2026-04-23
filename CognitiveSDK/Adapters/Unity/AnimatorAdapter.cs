using UnityEngine;
using CognitiveSDK.Runtime;

public class AnimatorAdapter : MonoBehaviour
{
    public Vector3 EvaluateScaleMultiplier(CognitiveResult result)
    {
        string actionType = result?.action_type ?? "idle";
        float dwellTime = Mathf.Max(0.5f, result?.dwell_time ?? 2f);
        float breath = Mathf.Sin(Time.time * (1.2f + 1f / dwellTime)) * 0.02f;

        switch (actionType)
        {
            case "approach":
                return new Vector3(1.02f, 1.01f + breath, 1.02f);
            case "withdraw":
                return new Vector3(0.98f, 1f + breath, 0.98f);
            case "move":
                return new Vector3(1f, 1f + breath * 0.7f, 1f);
            case "dwell":
                return new Vector3(1f, 1.01f + breath, 1f);
            default:
                return new Vector3(1f, 1f + breath * 0.5f, 1f);
        }
    }

    public Vector3 EvaluateRotationOffset(CognitiveResult result, float destinationBias)
    {
        string actionType = result?.action_type ?? "idle";
        float speed = Mathf.Clamp01(result?.action_speed ?? 0f);
        float sway = Mathf.Sin(Time.time * (1.8f + speed * 2.4f));

        switch (actionType)
        {
            case "approach":
                return new Vector3(-4f, destinationBias * 5f, sway * 1.5f);
            case "withdraw":
                return new Vector3(5f, -destinationBias * 4f, sway * 1.2f);
            case "move":
                return new Vector3(-2f, destinationBias * 3f, sway * 2f);
            case "dwell":
                return new Vector3(0f, destinationBias * 1.5f, sway * 0.6f);
            default:
                return new Vector3(0f, 0f, sway * 0.4f);
        }
    }
}
