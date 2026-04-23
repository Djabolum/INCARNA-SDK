using CognitiveSDK.Runtime;
using UnityEngine;

public class SDKSmokeTest : MonoBehaviour
{
    [Header("Profile")]
    public TextAsset profileJson;
    [TextArea]
    public string sdkTokenOverride = "";

    [Header("Smoke Test")]
    [TextArea]
    public string inputText = "Je te défie";
    public bool runOnStart = false;

    private CognitiveNPC npc;

    void Start()
    {
        if (runOnStart)
        {
            ExecuteSmokeTest();
        }
    }

    [ContextMenu("Run SDK Smoke Test")]
    public void ExecuteSmokeTest()
    {
        if (profileJson == null)
        {
            Debug.LogError("SDKSmokeTest: assign a profile JSON asset first.");
            return;
        }

        var profile = CognitiveProfile.LoadFromJson(profileJson.text);
        if (!string.IsNullOrWhiteSpace(sdkTokenOverride))
        {
            profile.sdk_token = sdkTokenOverride.Trim();
        }

        npc = new CognitiveNPC(profile);
        var result = npc.Step(inputText);

        if (result == null)
        {
            Debug.LogError("SDKSmokeTest: no result returned by npc.Step().");
            return;
        }

        Debug.Log($"[SDKSmokeTest] Input: {inputText}");
        Debug.Log($"[SDKSmokeTest] State={result.state} Gate={result.behavior_gate} Policy={result.runtime_policy}");
        Debug.Log($"[SDKSmokeTest] Stability={result.stability:F3} Aggression={result.aggression:F3} Pressure={result.pressure:F3} Control={result.control:F3}");
        Debug.Log($"[SDKSmokeTest] Fidelity={result.fidelity_score:F3} Confidence={result.behavior_confidence:F3} Alignment={result.alignment_state}");
        Debug.Log($"[SDKSmokeTest] Intent={result.intent}");
        Debug.Log($"[SDKSmokeTest] Text={result.text}");
    }
}
