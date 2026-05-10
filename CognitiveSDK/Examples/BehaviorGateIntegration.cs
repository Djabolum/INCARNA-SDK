/*
 * BehaviorGateIntegration.cs
 * Incarna SDK — behavior_gate integration sample
 *
 * Shows how to read and react to the gate signal in a Unity studio context.
 * The gate is advisory: you decide how to react. Incarna does not prescribe.
 *
 * "The gate holds the retina, not the wheel."
 */

using UnityEngine;

namespace Incarna.Examples
{
    public class BehaviorGateIntegration : MonoBehaviour
    {
        [Header("Incarna")]
        public CognitiveNPC npc;

        // ── Per-step integration ──────────────────────────────────────────────

        private void Update()
        {
            // Build your embodied request from the scene
            var request = BuildRequestFromScene();
            var result = npc.StepEmbodied(request);

            // The gate is the primary integration signal
            ApplyGate(result);

            // The NPC action is already decided by the runtime — just execute it
            ExecuteAction(result);
        }

        // ── Gate reaction — studio chooses how to respond ─────────────────────

        private void ApplyGate(CognitiveResult result)
        {
            switch (result.behavior_gate)
            {
                case "open":
                    // Full confidence: play normal animations, engage dialogue,
                    // allow all interactions.
                    SetNormalMode();
                    break;

                case "caution":
                    // Monitored: the runtime is flagging a context worth watching.
                    // Studio options (choose based on game design):
                    //   - slow down animations slightly
                    //   - avoid triggering high-stakes dialogue branches
                    //   - show a debug overlay in development builds
                    //   - log the step for analysis
                    SetCautionMode(result.gate_layer_studio_safe_reason);
                    if (Debug.isDebugBuild)
                        ShowDebugOverlay(result);
                    break;

                case "restricted":
                    // Low confidence: severe drift detected.
                    // Studio options (choose based on game design):
                    //   - restrict complex dialogue trees
                    //   - avoid escalating scene tension
                    //   - apply safe fallback animation/posture
                    //   - trigger a narrative transition if appropriate
                    SetRestrictedMode(result.gate_layer_studio_safe_reason);
                    break;
            }
        }

        // ── What the gate tells you ───────────────────────────────────────────

        private void LogGateContext(CognitiveResult result)
        {
            // These fields are always safe to read:
            Debug.Log($"[Gate] {result.behavior_gate} " +
                      $"| drift: {result.drift_layer_drift_class} ({result.drift_layer_severity}) " +
                      $"| source: {result.gate_layer_gate_source} " +
                      $"| reason: {result.gate_layer_studio_safe_reason}");

            // semantic_layer tells you if orientation data was available
            if (result.semantic_layer_degraded)
                Debug.LogWarning("[Gate] Semantic orientation unavailable — acting on fallback map.");
        }

        // ── Studio-defined reactions (implement to match your game) ───────────

        private void SetNormalMode() { /* your implementation */ }

        private void SetCautionMode(string reason)
        {
            // Example: reduce animation blend speed, log the reason
            Debug.Log($"[Incarna] Caution — {reason}");
        }

        private void SetRestrictedMode(string reason)
        {
            // Example: freeze high-stakes interactions, apply safe posture
            Debug.LogWarning($"[Incarna] Restricted — {reason}");
        }

        private void ShowDebugOverlay(CognitiveResult result)
        {
            // See: OrundraDebugHUD for a ready-to-use overlay
        }

        private void ExecuteAction(CognitiveResult result)
        {
            // action_type and destination_hint drive movement/animation
            // gate does NOT block action_policy — the NPC still acts
        }

        private EmbodiedStepRequest BuildRequestFromScene()
        {
            // See: Examples/StudioDemo/DemoController.cs for full implementation
            return new EmbodiedStepRequest();
        }
    }
}
