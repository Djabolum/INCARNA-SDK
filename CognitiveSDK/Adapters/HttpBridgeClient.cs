using System;
using System.Net.Http;
using System.Text;
using CognitiveSDK.Runtime;

namespace CognitiveSDK.Adapters
{
    public class HttpBridgeClient
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly CognitiveProfile profile;

        public HttpBridgeClient(CognitiveProfile profile)
        {
            this.profile = profile;
        }

        public CognitiveResult Process(string input)
        {
            try
            {
                var payload = new CognitiveBridgeRequest
                {
                    input = input,
                    profile = profile,
                };

                var json = UnityEngine.JsonUtility.ToJson(payload);
                var response = Client.PostAsync(
                    profile.bridge_url,
                    new StringContent(json, Encoding.UTF8, "application/json")
                ).GetAwaiter().GetResult();

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    return FallbackResult("unstable", 0.22f, 0.65f, "bridge_error");
                }

                return MapBridgeResponse(UnityEngine.JsonUtility.FromJson<CognitiveBridgeResponse>(body));
            }
            catch (Exception)
            {
                return FallbackResult("unstable", 0.2f, 0.7f, "network_fallback");
            }
        }

        public CognitiveResult ProcessEmbodied(EmbodiedStepRequest request)
        {
            try
            {
                var json = UnityEngine.JsonUtility.ToJson(request);
                var response = Client.PostAsync(
                    profile.bridge_url,
                    new StringContent(json, Encoding.UTF8, "application/json")
                ).GetAwaiter().GetResult();

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    return FallbackResult("unstable", 0.22f, 0.65f, "embodied_bridge_error");
                }

                return MapBridgeResponse(UnityEngine.JsonUtility.FromJson<CognitiveBridgeResponse>(body));
            }
            catch (Exception)
            {
                return FallbackResult("unstable", 0.2f, 0.7f, "embodied_network_fallback");
            }
        }

        public bool ResetSession()
        {
            try
            {
                var payload = new CognitiveBridgeRequest
                {
                    input = string.Empty,
                    profile = profile,
                };

                var json = UnityEngine.JsonUtility.ToJson(payload);
                var resetUrl = profile.bridge_url.Replace("/step", "/reset-session");
                var response = Client.PostAsync(
                    resetUrl,
                    new StringContent(json, Encoding.UTF8, "application/json")
                ).GetAwaiter().GetResult();

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public InspectResponse InspectEmbodied(string sessionId, string npcId)
        {
            try
            {
                var payload = new InspectRequest
                {
                    session_id = sessionId,
                    npc_id = npcId,
                };

                var json = UnityEngine.JsonUtility.ToJson(payload);
                var inspectUrl = profile.bridge_url.Replace("/step", "/inspect");
                var response = Client.PostAsync(
                    inspectUrl,
                    new StringContent(json, Encoding.UTF8, "application/json")
                ).GetAwaiter().GetResult();

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return UnityEngine.JsonUtility.FromJson<InspectResponse>(body);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static CognitiveResult MapBridgeResponse(CognitiveBridgeResponse bridgeResponse)
        {
            return new CognitiveResult
            {
                state = string.IsNullOrWhiteSpace(bridgeResponse.state) ? CognitiveState.Neutral : bridgeResponse.state,
                stability = bridgeResponse.stability > 0 ? bridgeResponse.stability : bridgeResponse.signals != null ? bridgeResponse.signals.stability : 0f,
                aggression = bridgeResponse.aggression > 0 ? bridgeResponse.aggression : bridgeResponse.signals != null ? bridgeResponse.signals.aggression : 0f,
                pressure = bridgeResponse.pressure > 0 ? bridgeResponse.pressure : bridgeResponse.signals != null ? bridgeResponse.signals.pressure : 0f,
                control = bridgeResponse.control > 0 ? bridgeResponse.control : bridgeResponse.signals != null ? bridgeResponse.signals.control : 0f,
                fidelity_score = bridgeResponse.fidelity_score > 0 ? bridgeResponse.fidelity_score : bridgeResponse.signals != null ? bridgeResponse.signals.fidelity_score : 0f,
                behavior_confidence = bridgeResponse.behavior_confidence,
                alignment_state = string.IsNullOrWhiteSpace(bridgeResponse.alignment_state) ? "unknown" : bridgeResponse.alignment_state,
                behavior_gate = string.IsNullOrWhiteSpace(bridgeResponse.behavior_gate) ? "caution" : bridgeResponse.behavior_gate,
                runtime_policy = string.IsNullOrWhiteSpace(bridgeResponse.runtime_policy) ? "direct_runtime" : bridgeResponse.runtime_policy,
                intent = string.IsNullOrWhiteSpace(bridgeResponse.intent) ? "react" : bridgeResponse.intent,
                text = bridgeResponse.text,
                action_type = bridgeResponse.action != null ? bridgeResponse.action.type : null,
                target_affordance_id = bridgeResponse.action != null ? bridgeResponse.action.target_affordance_id : null,
                destination_hint = bridgeResponse.action != null ? bridgeResponse.action.destination_hint : null,
                action_speed = bridgeResponse.action != null ? bridgeResponse.action.speed : 0f,
                dwell_time = bridgeResponse.action != null ? bridgeResponse.action.dwell_time : 0f,
                action_animation = bridgeResponse.action != null ? bridgeResponse.action.animation : null,
                thermal_comfort = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.thermal_comfort : 0f,
                energy = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.energy : 0f,
                stress_load = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.stress_load : 0f,
                safety_feeling = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.safety_feeling : 0f,
                social_attunement = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.social_attunement : 0f,
                curiosity_drive = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.curiosity_drive : 0f,
                valence = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.valence : 0f,
                arousal = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.arousal : 0f,
                comfort_index = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.comfort_index : 0f,
                regulation_pressure = bridgeResponse.internal_state != null ? bridgeResponse.internal_state.regulation_pressure : 0f,
                current_zone_bias = bridgeResponse.memory != null ? bridgeResponse.memory.current_zone_bias : 0f,
                target_zone_bias = bridgeResponse.memory != null ? bridgeResponse.memory.target_zone_bias : 0f,
                recent_reward_trace = bridgeResponse.memory != null ? bridgeResponse.memory.recent_reward_trace : 0f,
                dominant_need = bridgeResponse.explain != null ? bridgeResponse.explain.dominant_need : null,
                dominant_pull = bridgeResponse.explain != null ? bridgeResponse.explain.dominant_pull : null,
                dominant_risk = bridgeResponse.explain != null ? bridgeResponse.explain.dominant_risk : null,
                continuity = bridgeResponse.continuity_info != null ? bridgeResponse.continuity_info.continuity : 0f,
                session_id = bridgeResponse.session != null ? bridgeResponse.session.session_id : null,
                npc_id = bridgeResponse.session != null ? bridgeResponse.session.npc_id : null,
            };
        }

        private static CognitiveResult FallbackResult(string state, float stability, float aggression, string intent)
        {
            return new CognitiveResult
            {
                state = state,
                stability = stability,
                aggression = aggression,
                pressure = 0.72f,
                control = 0.24f,
                fidelity_score = 0.22f,
                behavior_confidence = 0.28f,
                alignment_state = "unknown",
                behavior_gate = "restricted",
                runtime_policy = "fallback_to_safe_behavior",
                intent = intent,
                text = string.Empty,
                action_type = "idle",
            };
        }
    }
}
