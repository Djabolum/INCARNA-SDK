# StepResponse — Public Schema

This document describes the fields returned by `CognitiveNPC.StepEmbodied()`.

Only studio-facing fields are listed here. Internal runtime state is not exposed.

---

## Core Fields

```csharp
public string state;             // "settling" | "adapting" | "strained"
public string alignment_state;   // "aligned" | "partial" | "drifting"
public string runtime_policy;    // active homeostasis policy
public string behavior_gate;     // "open" | "caution" | "restricted"  ← key signal
public float  behavior_confidence; // [0-1] confidence in this step
```

## Action

```csharp
public string action_type;               // "move" | "dwell" | "approach" | "withdraw" | "idle"
public string target_affordance_id;      // target in the scene
public string destination_hint;          // "rest_zone" | "shade_zone" | "human_anchor" | …
public float  action_speed;              // [0-1]
public float  dwell_time;               // seconds if action_type == "dwell"
```

## Behavioral Signals

```csharp
public float stability;         // [0-1] overall coherence
public float aggression;        // [0-1]
public float pressure;          // [0-1] studio-safe aggregate pressure
public float control;           // [0-1] self-regulation
public float fidelity_score;    // [0-1] fidelity to role profile
```

## Gate Layer

```csharp
// gate_layer is the complete advisory surface
public GateLayerDto gate_layer;

gate_layer.behavior_gate;      // "open" | "caution" | "restricted"
gate_layer.gate_source;        // "runtime_policy" | "drift_layer"
gate_layer.drift_gate;         // raw drift gate before maxGate
gate_layer.studio_safe_reason; // human-readable reason
gate_layer.version;            // "behavior_gate_v1"
```

## Drift Layer

```csharp
// drift_layer is the named behavioral gap (V1, read-only)
public DriftLayerDto drift_layer;

drift_layer.drift_class;        // see drift class reference below
drift_layer.severity;           // "none" | "mild" | "moderate" | "significant"
drift_layer.confidence;         // [0-1]
drift_layer.degraded_semantics;
drift_layer.studio_safe_reason;
drift_layer.context_size;       // window steps used (0 = instant only)
```

## Semantic Layer

```csharp
public SemanticLayerDto semantic_layer;

semantic_layer.source;     // "elysian" | "fallback" | "none"
semantic_layer.confidence; // [0-1]
semantic_layer.degraded;   // true = orientation unavailable
```

## Morphology Projection (optional, visual-only)

```csharp
public OrundraMorphologyCandidateDto morphology_candidate;
public MorphologyProjectionReceiptDto morphology_projection;
public bool morphology_candidate_admitted;
```

The candidate is exposed only when its schema, source, session, entity,
bounded visual fields, and all four false authority bits pass the SDK guard.
It is rejected unless the projection receipt is valid and has
`status="produced"`.

`morphology_projection.status="produced"` describes the source process.
`morphology_candidate_admitted` describes the SDK output decision. They are
kept distinct intentionally.

```text
body_state_exposed      = false
influences_action       = false
behavior_override       = false
stable_memory_write     = false
canon_promotion_allowed = false
```

Raw homeostasis, appraisal, memory weights, explanatory internals, Quark
events, and Elysian debug payloads are not part of `StepEmbodied()`.

---

## Drift Class Reference

| Class | Meaning |
|-------|---------|
| `stable_embodiment` | No drift detected |
| `semantic_degraded` | Orientation unavailable (Elysian fallback) |
| `loop_collapse` | Repeated action without progression |
| `agency_fade` | Low-value motion, reduced self-direction |
| `player_mirroring` | Role axis diluted by player presence |
| `role_overstretch` | Internal pressure above scene context |
| `quest_drift` | Objective alignment unstable |

---

## Gate Contract

```
behavior_gate is advisory only.
It does not alter action_policy.
The gate holds the retina, not the wheel.
```

*Version: studio_contract_v0_7 — 2026-08-22*
