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
public float pressure;          // [0-1] internal regulation pressure
public float control;           // [0-1] self-regulation
public float fidelity_score;    // [0-1] fidelity to role profile
```

## Gate Layer

```csharp
// gate_layer is the complete advisory surface
public string gate_layer_behavior_gate;      // "open" | "caution" | "restricted"
public string gate_layer_gate_source;        // "runtime_policy" | "drift_layer"
public string gate_layer_drift_gate;         // raw drift gate before maxGate
public string gate_layer_studio_safe_reason; // human-readable reason
public string gate_layer_version;            // "behavior_gate_v1"
```

## Drift Layer

```csharp
// drift_layer is the named behavioral gap (V1, read-only)
public string drift_layer_drift_class;  // see drift class reference below
public string drift_layer_severity;     // "none" | "mild" | "moderate" | "significant"
public float  drift_layer_confidence;   // [0-1]
public bool   drift_layer_degraded_semantics;
public string drift_layer_studio_safe_reason;
public int    drift_layer_context_size; // window steps used (0 = instant only)
```

## Semantic Layer

```csharp
public string semantic_layer_source;     // "elysian" | "fallback" | "none"
public float  semantic_layer_confidence; // [0-1]
public bool   semantic_layer_degraded;   // true = orientation unavailable
```

## Internal State (informative, read-only)

```csharp
public float thermal_comfort;   // [0-1]
public float energy;            // [0-1]
public float stress_load;       // [0-1]
public float safety_feeling;    // [0-1]
public float social_attunement; // [0-1]
public float curiosity_drive;   // [0-1]
```

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

*Version: behavior_gate_v1 — 2026-05-10*
