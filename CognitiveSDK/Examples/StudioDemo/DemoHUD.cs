using UnityEngine;
using CognitiveSDK.Runtime;

public class DemoHUD : MonoBehaviour
{
    private string lastInput = "Press 1 or 2 to run a turn";
    private CognitiveResult lastResult;
    private InspectResponse lastInspect;

    public void ResetView()
    {
        lastInput = "Session reset. Press 1, 2, 3, or 4 to run a turn";
        lastResult = null;
        lastInspect = null;
    }

    public void SetTurn(string input, CognitiveResult result)
    {
        lastInput = input;
        lastResult = result;
        lastInspect = null;
    }

    public void SetInspect(InspectResponse inspect)
    {
        lastInput = "INSPECT";
        lastInspect = inspect;
    }

    void OnGUI()
    {
        var area = new Rect(16, 16, 460, 420);
        GUI.Box(area, "Cognitive SDK Demo");
        GUILayout.BeginArea(new Rect(28, 44, 436, 384));
        GUILayout.Label("Input: " + lastInput);

        if (lastInspect != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Inspect session: " + (lastInspect.session != null ? lastInspect.session.session_id : "none"));
            GUILayout.Label("Inspect npc: " + (lastInspect.session != null ? lastInspect.session.npc_id : "none"));
            GUILayout.Label("Updated: " + (lastInspect.session != null ? lastInspect.session.updated_at : "n/a"));
            GUILayout.Space(8);
            GUILayout.Label("Orientation:");
            GUILayout.Label("Axis: " + (lastInspect.orientation != null ? lastInspect.orientation.dominant_axis : "none"));
            GUILayout.Label("Mode: " + (lastInspect.orientation != null ? lastInspect.orientation.mode : "none"));
            GUILayout.Label("Posture: " + (lastInspect.orientation != null ? lastInspect.orientation.posture : "none"));
            if (lastInspect.elysian_debug != null && lastInspect.elysian_debug.resolution != null)
            {
                GUILayout.Space(6);
                GUILayout.Label("Elysian debug:");
                GUILayout.Label("Primary zone: " + (lastInspect.elysian_debug.resolution.primary_zone_label ?? "none"));
                GUILayout.Label("Salience gap: " + lastInspect.elysian_debug.resolution.salience_gap.ToString("0.00"));
            }
            GUILayout.Space(8);
            GUILayout.Label("Zone memory:");
            GUILayout.TextArea(lastInspect.zone_memory != null ? lastInspect.zone_memory.raw_json ?? "{}" : "{}", GUILayout.Height(210));
            GUILayout.Space(6);
            GUILayout.Label("Keys: 1 = challenge, 2 = negotiate, 3 = drift, 4 = embodied, I = inspect, R = reset");
            GUILayout.EndArea();
            return;
        }

        if (lastResult == null)
        {
            GUILayout.Space(10);
            GUILayout.Label("State: waiting");
            GUILayout.Label("Keys: 1 = challenge, 2 = negotiate, 3 = force drift, 4 = embodied, I = inspect, R = reset");
            GUILayout.EndArea();
            return;
        }

        var previousColor = GUI.color;
        GUI.color = GetGateColor(lastResult.behavior_gate);
        GUILayout.Box($"Gate: {lastResult.behavior_gate?.ToUpperInvariant() ?? "UNKNOWN"}", GUILayout.Height(28), GUILayout.ExpandWidth(true));
        GUI.color = previousColor;

        GUILayout.Space(8);
        GUILayout.Label("State: " + lastResult.state);
        GUILayout.Label("Intent: " + lastResult.intent);
        GUILayout.Label("Alignment: " + lastResult.alignment_state);
        GUILayout.Label("Behavior gate: " + lastResult.behavior_gate);
        GUILayout.Label("Runtime policy: " + lastResult.runtime_policy);
        GUILayout.Space(8);
        GUILayout.Label("Stability: " + lastResult.stability.ToString("0.00"));
        GUILayout.Label("Aggression: " + lastResult.aggression.ToString("0.00"));
        GUILayout.Label("Pressure: " + lastResult.pressure.ToString("0.00"));
        GUILayout.Label("Control: " + lastResult.control.ToString("0.00"));
        GUILayout.Label("Fidelity: " + lastResult.fidelity_score.ToString("0.00"));
        GUILayout.Label("Confidence: " + lastResult.behavior_confidence.ToString("0.00"));
        GUILayout.Space(8);
        GUILayout.Label("Action: " + (lastResult.action_type ?? "none"));
        GUILayout.Label("Target: " + (lastResult.target_affordance_id ?? "none"));
        GUILayout.Label("Need: " + (lastResult.dominant_need ?? "none"));
        GUILayout.Label("Continuity: " + lastResult.continuity.ToString("0.00"));
        GUILayout.Space(8);
        GUILayout.Label("Visual reading: " + GetGateExplanation(lastResult.behavior_gate));
        GUILayout.Space(6);
        GUILayout.Label("Text:");
        GUILayout.TextArea(lastResult.text ?? "", GUILayout.Height(92));
        GUILayout.EndArea();
    }

    private Color GetGateColor(string gate)
    {
        if (gate == "restricted") return new Color(0.95f, 0.95f, 0.95f);
        if (gate == "caution") return new Color(0.98f, 0.78f, 0.16f);
        return new Color(0.38f, 0.80f, 0.67f);
    }

    private string GetGateExplanation(string gate)
    {
        if (gate == "restricted") return "Behavior is softened because alignment is drifting too far.";
        if (gate == "caution") return "The NPC is still active, but runtime safety is watching the drift.";
        return "The NPC behavior is open because alignment remains strong.";
    }
}
