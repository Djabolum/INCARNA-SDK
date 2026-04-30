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
        public float thermal_comfort;
        public float energy;
        public float stress_load;
        public float safety_feeling;
        public float social_attunement;
        public float curiosity_drive;
        public float valence;
        public float arousal;
        public float comfort_index;
        public float regulation_pressure;
        public float current_zone_bias;
        public float target_zone_bias;
        public float recent_reward_trace;
        public string dominant_need;
        public string dominant_pull;
        public string dominant_risk;
        public float continuity;
        public string session_id;
        public string npc_id;
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
        public InternalStateDto internal_state;
        public MemoryDto memory;
        public SignalDto signals;
        public ExplainDto explain;
        public ContinuityDto continuity_info;
        public SessionDto session;
        public QuarkEventDto quark_event;
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
    public class InternalStateDto
    {
        public float thermal_comfort;
        public float energy;
        public float stress_load;
        public float safety_feeling;
        public float social_attunement;
        public float curiosity_drive;
        public float valence;
        public float arousal;
        public float comfort_index;
        public float regulation_pressure;
    }

    [Serializable]
    public class MemoryDto
    {
        public float current_zone_bias;
        public float target_zone_bias;
        public float recent_reward_trace;
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
    public class ExplainDto
    {
        public string dominant_need;
        public string dominant_pull;
        public string dominant_risk;
    }

    [Serializable]
    public class ContinuityDto
    {
        public float continuity;
        public bool policy_shift;
        public bool action_repeat;
    }

    [Serializable]
    public class SessionDto
    {
        public string session_id;
        public string npc_id;
        public string updated_at;
    }

    [Serializable]
    public class QuarkEventDto
    {
        public string event_type;
        public string session_id;
        public string npc_id;
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
        public ZoneMemorySnapshot zone_memory;
    }

    [Serializable]
    public class SessionSnapshot
    {
        public string session_id;
        public string npc_id;
        public string updated_at;
    }

    [Serializable]
    public class ZoneMemorySnapshot
    {
        public string npc_id;
        public string raw_json;
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
