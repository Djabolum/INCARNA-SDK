using System;

namespace CognitiveSDK.Runtime
{
    [Serializable]
    public class CognitiveResult
    {
        public string state;
        public float stability;
        public float aggression;
        public float pressure;
        public float control;
        public float fidelity_score;
        public float behavior_confidence;
        public string alignment_state;
        public string behavior_gate;
        public string runtime_policy;
        public string intent;
        public string text;
        public string action_type;
        public string target_affordance_id;
        public string destination_hint;
        public float action_speed;
        public float dwell_time;
        public string action_animation;
        public float continuity;
        public string session_id;
        public string npc_id;
        public SemanticLayerDto semantic_layer;
        public DriftLayerDto drift_layer;
        public GateLayerDto gate_layer;
        public PresenceProjectionDto presence_projection;
        public OrundraMorphologyCandidateDto morphology_candidate;
        public MorphologyProjectionReceiptDto morphology_projection;
        public bool morphology_candidate_admitted;
    }

    [Serializable]
    public class CognitiveProfile
    {
        public string fossil_id;
        public string quark_id;
        public string mode = "remote";
        public string npc_name = "NPC";
        public string bridge_url = "https://game-ai.cordee.ovh/api/v1/sdk/step";
        public string sdk_token = "";
        public string npc_id = "incarna-alpha";
        public string session_id = "unity-demo";
        public bool embodiment_enabled = false;

        public static CognitiveProfile LoadFromJson(string json)
        {
            return UnityEngine.JsonUtility.FromJson<CognitiveProfile>(json);
        }
    }

    [Serializable]
    public class CognitiveBridgeRequest
    {
        public string input;
        public CognitiveProfile profile;
    }

    [Serializable]
    public class CognitiveBridgeResponse
    {
        public string state;
        public float stability;
        public float aggression;
        public float pressure;
        public float control;
        public float fidelity_score;
        public float behavior_confidence;
        public string alignment_state;
        public string behavior_gate;
        public string runtime_policy;
        public string intent;
        public string text;
        public ActionDto action;
        public SignalDto signals;
        public ContinuityDto continuity;
        public SemanticLayerDto semantic_layer;
        public DriftLayerDto drift_layer;
        public GateLayerDto gate_layer;
        public PresenceProjectionDto presence_projection;
        public OrundraMorphologyCandidateDto morphology_candidate;
        public MorphologyProjectionReceiptDto morphology_projection;
        public SessionDto session;
    }

    [Serializable]
    public class WorldStateDto
    {
        public string zone_id;
        public float time_of_day;
        public float solar_exposure;
        public float ambient_temp;
        public float shade_distance;
        public float rest_zone_distance;
        public float human_distance;
        public float human_familiarity;
        public float noise_level;
        public float safety_index;
        public float novelty_index;
    }

    [Serializable]
    public class AffordanceDto
    {
        public string id;
        public string type;
        public float distance;
        public float quality;
    }

    [Serializable]
    public class LastActionResultDto
    {
        public string action;
        public float success;
        public float cost;
    }

    [Serializable]
    public class EmbodiedStepRequest
    {
        public string session_id;
        public string npc_id;
        public long timestamp_ms;
        public float dt;
        public WorldStateDto world;
        public AffordanceDto[] affordances;
        public LastActionResultDto last_action_result;
    }

    [Serializable]
    public class ActionDto
    {
        public string type;
        public string target_affordance_id;
        public string destination_hint;
        public float speed;
        public float dwell_time;
        public string animation;
    }

    [Serializable]
    public class SemanticLayerDto
    {
        public string source;
        public float confidence;
        public bool degraded;
        public string reason;
    }

    [Serializable]
    public class DriftLayerDto
    {
        public string drift_class;
        public string severity;
        public float confidence;
        public bool degraded_semantics;
        public string studio_safe_reason;
        public int context_size;
    }

    [Serializable]
    public class SignalDto
    {
        public float stability;
        public float aggression;
        public float pressure;
        public float control;
        public float fidelity_score;
    }

    [Serializable]
    public class GateLayerDto
    {
        public string behavior_gate;
        public string gate_source;
        public string drift_gate;
        public string studio_safe_reason;
        public string version;
    }

    [Serializable]
    public class ContinuityDto
    {
        public float continuity;
        public bool policy_shift;
        public bool action_repeat;
    }

    [Serializable]
    public class PresenceProjectionDto
    {
        public string mode;
        public bool influences_action;
        public bool behavior_override;
        public bool stable_memory_write;
    }

    [Serializable]
    public class MorphologyAuthorityDto
    {
        public bool runtime_body_change;
        public bool stable_body_change;
        public bool memory_write;
        public bool promotion_to_canon;
    }

    [Serializable]
    public class MorphologySignalsDto
    {
        public float cosmic_void_phase;
        public float body_formation_progress;
        public float trust_construction_progress;
        public float shelter_density;
        public float warmth_exposure;
        public float shadow_affinity;
        public float chaos_pressure;
        public float novelty_pressure;
        public float neoxys_density;
    }

    [Serializable]
    public class MorphologyMaterializationZoneDto
    {
        public string region;
        public bool materialized;
        public float strength;
        public string reason;
    }

    [Serializable]
    public class MorphologyCandidateEffectDto
    {
        public string region;
        public string effect;
        public float strength;
        public string reason;
    }

    [Serializable]
    public class OrundraMorphologyCandidateDto
    {
        public string schema;
        public string session_id;
        public string trace_id;
        public string source;
        public string entity_id;
        public string persistence;
        public string phase;
        public MorphologyAuthorityDto authority;
        public MorphologySignalsDto signals;
        public MorphologyMaterializationZoneDto[] materialization_zones;
        public MorphologyCandidateEffectDto[] candidate_effects;
        public bool fossilizable;
        public bool canon_promotion_allowed;
    }

    [Serializable]
    public class MorphologyProjectionReceiptDto
    {
        public string schema;
        public string status;
        public string reason;
        public string source;
        public string mode;
        public bool body_state_exposed;
        public bool influences_action;
        public bool behavior_override;
        public bool stable_memory_write;
        public bool canon_promotion_allowed;
    }

    [Serializable]
    public class SessionDto
    {
        public string session_id;
        public string npc_id;
        public string updated_at;
    }

    [Serializable]
    public class InspectRequest
    {
        public string session_id;
        public string npc_id;
    }

    [Serializable]
    public class InspectResponse
    {
        public SessionSnapshot session;
        public OrientationSnapshot orientation;
        public ElysianDebugSnapshot elysian_debug;
    }

    [Serializable]
    public class SessionSnapshot
    {
        public string session_id;
        public string npc_id;
        public string updated_at;
    }

    [Serializable]
    public class OrientationSnapshot
    {
        public string dominant_axis;
        public string secondary_axis;
        public float world_readability;
        public float interpretive_confidence;
        public string mode;
        public string posture;
    }

    [Serializable]
    public class ElysianDebugSnapshot
    {
        public ResolutionSnapshot resolution;
    }

    [Serializable]
    public class ResolutionSnapshot
    {
        public string primary_zone_id;
        public string primary_zone_label;
        public float primary_zone_salience;
        public string secondary_zone_id;
        public string secondary_zone_label;
        public float secondary_zone_salience;
        public float salience_gap;
        public string recent_vector;
        public string[] interpretation_notes;
    }
}
