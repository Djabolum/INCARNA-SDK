using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var request = new HttpRequestMessage(HttpMethod.Post, profile.bridge_url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrWhiteSpace(profile.sdk_token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profile.sdk_token);
                }
                var response = Client.SendAsync(
                    request
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
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, profile.bridge_url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrWhiteSpace(profile.sdk_token))
                {
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profile.sdk_token);
                }
                var response = Client.SendAsync(
                    httpRequest
                ).GetAwaiter().GetResult();

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    return FallbackResult("unstable", 0.22f, 0.65f, "embodied_bridge_error");
                }

                return MapBridgeResponse(
                    UnityEngine.JsonUtility.FromJson<CognitiveBridgeResponse>(body),
                    request != null ? request.session_id : null,
                    request != null ? request.npc_id : null
                );
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
                var request = new HttpRequestMessage(HttpMethod.Post, resetUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrWhiteSpace(profile.sdk_token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profile.sdk_token);
                }
                var response = Client.SendAsync(
                    request
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
                var request = new HttpRequestMessage(HttpMethod.Post, inspectUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrWhiteSpace(profile.sdk_token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profile.sdk_token);
                }
                var response = Client.SendAsync(
                    request
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

        private static CognitiveResult MapBridgeResponse(
            CognitiveBridgeResponse bridgeResponse,
            string expectedSessionId = null,
            string expectedNpcId = null)
        {
            if (bridgeResponse == null)
            {
                return FallbackResult("unstable", 0.2f, 0.7f, "invalid_bridge_response");
            }

            var safeMorphologyProjection = MorphologyContractGuard.IsSafeProjection(
                bridgeResponse.morphology_projection
            ) ? bridgeResponse.morphology_projection : null;
            var safeMorphologyCandidate = safeMorphologyProjection != null &&
                safeMorphologyProjection.status == "produced" &&
                MorphologyContractGuard.IsSafeCandidate(
                    bridgeResponse.morphology_candidate,
                    expectedSessionId,
                    expectedNpcId
                ) ? bridgeResponse.morphology_candidate : null;
            var safePresenceProjection = bridgeResponse.presence_projection != null &&
                !bridgeResponse.presence_projection.influences_action &&
                !bridgeResponse.presence_projection.behavior_override &&
                !bridgeResponse.presence_projection.stable_memory_write
                ? bridgeResponse.presence_projection
                : null;

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
                continuity = bridgeResponse.continuity != null ? bridgeResponse.continuity.continuity : 0f,
                session_id = bridgeResponse.session != null ? bridgeResponse.session.session_id : null,
                npc_id = bridgeResponse.session != null ? bridgeResponse.session.npc_id : null,
                semantic_layer = bridgeResponse.semantic_layer,
                drift_layer = bridgeResponse.drift_layer,
                gate_layer = bridgeResponse.gate_layer,
                presence_projection = safePresenceProjection,
                morphology_candidate = safeMorphologyCandidate,
                morphology_projection = safeMorphologyProjection,
                morphology_candidate_admitted = safeMorphologyCandidate != null,
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
