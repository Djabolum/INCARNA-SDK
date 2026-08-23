using System.Reflection;
using CognitiveSDK.Adapters;
using CognitiveSDK.Runtime;

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static MorphologyProjectionReceiptDto Projection(string status = "produced")
{
    return new MorphologyProjectionReceiptDto
    {
        schema = "orundra_morphology_projection_receipt_v0",
        status = status,
        source = "incarna.internal_to_elysian",
        mode = "post_decision_visual_only",
        body_state_exposed = false,
        influences_action = false,
        behavior_override = false,
        stable_memory_write = false,
        canon_promotion_allowed = false,
    };
}

static OrundraMorphologyCandidateDto Candidate()
{
    return new OrundraMorphologyCandidateDto
    {
        schema = "orundra_morphology_candidate_v0",
        session_id = "studio-session-v0-7",
        trace_id = "trace-v0-7",
        source = "elysian.morphing",
        entity_id = "orundra",
        persistence = "session_only",
        phase = "condensation",
        authority = new MorphologyAuthorityDto(),
        signals = new MorphologySignalsDto
        {
            cosmic_void_phase = 0.58f,
            body_formation_progress = 0.51f,
            trust_construction_progress = 0.49f,
            shelter_density = 0.62f,
            warmth_exposure = 0.44f,
            shadow_affinity = 0.38f,
            chaos_pressure = 0.12f,
            novelty_pressure = 0.24f,
            neoxys_density = 0.39f,
        },
        materialization_zones = new[]
        {
            new MorphologyMaterializationZoneDto
            {
                region = "hands",
                materialized = true,
                strength = 0.74f,
                reason = "contract test zone",
            }
        },
        candidate_effects = new[]
        {
            new MorphologyCandidateEffectDto
            {
                region = "post_auricular",
                effect = "thermal_groove_emergence",
                strength = 0.62f,
                reason = "contract test effect",
            }
        },
        fossilizable = true,
        canon_promotion_allowed = false,
    };
}

var candidate = Candidate();
Assert(MorphologyContractGuard.IsSafeCandidate(candidate, "studio-session-v0-7", "orundra"),
    "safe candidate should pass");
Assert(MorphologyContractGuard.IsSafeProjection(Projection()),
    "safe projection should pass");

candidate.authority.runtime_body_change = true;
Assert(!MorphologyContractGuard.IsSafeCandidate(candidate, "studio-session-v0-7", "orundra"),
    "authority must fail closed");
candidate.authority.runtime_body_change = false;
candidate.signals.chaos_pressure = float.NaN;
Assert(!MorphologyContractGuard.IsSafeCandidate(candidate, "studio-session-v0-7", "orundra"),
    "non-finite signals must fail closed");
candidate.signals.chaos_pressure = 0.12f;
Assert(!MorphologyContractGuard.IsSafeCandidate(candidate, "another-session", "orundra"),
    "session mismatch must fail closed");

var unsafeProjection = Projection();
unsafeProjection.influences_action = true;
Assert(!MorphologyContractGuard.IsSafeProjection(unsafeProjection),
    "action influence must fail closed");

foreach (var forbidden in new[] {
    "thermal_comfort", "energy", "stress_load", "safety_feeling", "social_attunement",
    "curiosity_drive", "valence", "arousal", "comfort_index", "regulation_pressure",
    "current_zone_bias", "target_zone_bias", "recent_reward_trace", "dominant_need",
    "dominant_pull", "dominant_risk"
})
{
    Assert(typeof(CognitiveResult).GetField(forbidden) == null,
        $"CognitiveResult must not expose {forbidden}");
}
foreach (var forbidden in new[] { "internal_state", "memory", "explain", "quark_event" })
{
    Assert(typeof(CognitiveBridgeResponse).GetField(forbidden) == null,
        $"CognitiveBridgeResponse must not expose {forbidden}");
}
Assert(typeof(InspectResponse).GetField("zone_memory") == null,
    "InspectResponse must not expose server-side memory payloads");

var map = typeof(HttpBridgeClient).GetMethod(
    "MapBridgeResponse",
    BindingFlags.NonPublic | BindingFlags.Static
);
Assert(map != null, "studio mapper should exist");
var response = new CognitiveBridgeResponse
{
    state = "adapting",
    behavior_gate = "open",
    action = new ActionDto { type = "dwell" },
    morphology_candidate = Candidate(),
    morphology_projection = Projection(),
    session = new SessionDto { session_id = "studio-session-v0-7", npc_id = "orundra" },
};
var mapped = (CognitiveResult)map.Invoke(
    null,
    new object[] { response, "studio-session-v0-7", "orundra" }
);
Assert(mapped.morphology_candidate != null, "safe candidate should reach CognitiveResult");
Assert(mapped.morphology_candidate_admitted, "safe candidate admission should be explicit");
response.morphology_candidate.authority.memory_write = true;
mapped = (CognitiveResult)map.Invoke(
    null,
    new object[] { response, "studio-session-v0-7", "orundra" }
);
Assert(mapped.morphology_candidate == null, "unsafe candidate must be filtered by mapper");
Assert(!mapped.morphology_candidate_admitted, "rejected candidate admission should be explicit");

Console.WriteLine("PASS_STUDIO_CONTRACT_V0_7");
