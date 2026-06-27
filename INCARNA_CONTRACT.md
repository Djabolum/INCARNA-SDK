# Incarna — Studio Contract

## What Incarna is

Incarna is an embodied NPC runtime. It produces one behavioral step at a time,
driven by the NPC's internal state, its spatial context, and its memory of the
environment.

Each step returns an action, a set of observable signals, and a behavior gate.

## What the studio receives

```
behavior_gate     — open / caution / restricted
drift_class       — named behavioral gap (stable_embodiment, loop_collapse, …)
drift_severity    — none / mild / moderate / significant
studio_safe_reason — human-readable context
action            — move / dwell / approach / withdraw / idle
action_target     — affordance the NPC is oriented toward
signals           — stability, fidelity_score, aggression, pressure, control
semantic_layer    — orientation source and confidence
```

## What the studio does not receive

- Raw homeostasis (stress_load, thermal_comfort, curiosity_drive…)
- Internal appraisal (arousal, regulation_pressure…)
- Session memory weights
- Fossil profile internals
- Invariant structure

## The gate contract

```
behavior_gate is advisory only.
It does not alter action_policy.
The gate holds the retina, not the wheel.
```

The studio decides what `caution` or `restricted` means in its game context.
Incarna surfaces the reading. The studio authors the reaction.

## The three states

| Gate | Meaning | Studio guidance |
|------|---------|-----------------|
| `open` | Coherent step | Full integration |
| `caution` | Flagged context | Log, slow, monitor |
| `restricted` | Severe drift | Safe fallback, restrict escalation |

## The architecture

```
Elysian           → builds a meaning map from the scene
Incarna runtime   → decides the action
Drift classifier  → observes the gap
Gate mapper       → translates gap → studio signal
QPL               → traces the observation
Genome            → reads global coherence
```

No layer reaches back into action_policy.  
Observation is separated from intervention.

## Orundra presence boundary

Orundra Presence Contract V0 is documented in:

```text
docs/PRESENCE_STACK_STATUS.md
docs/ORUNDRA_PRESENCE_CONTRACT_V0.md
docs/ORUNDRA_PRESENCE_STATE_MACHINE_V0.md
docs/ORUNDRA_PRESENCE_RUNTIME_PROJECTION_V0.md
docs/ORUNDRA_VOICE_READINESS_V0.md
docs/VOICE_SURFACE_CONTRACT_V0.md
docs/ORUNDRA_VOICE_INPUT_CONTRACT_V0.md
docs/ORUNDRA_VOICE_LOOP_READINESS_V0.md
```

The contract keeps expression separate from decision:

```text
orientation != presence
presence != voice
voice != action
```

Incarna can return the behavioral orientation. The presence layer decides how
that orientation is expressed, silenced, clarified, accompanied or withdrawn.

## Demonstration surface

```
GET  https://game-ai.cordee.ovh            — public sandbox
POST /api/v1/sdk/step                      — embodied step
POST /api/v1/sdk/reset-session             — session reset
POST /api/v1/sdk/inspect                   — debug memory state
```

Authentication: Bearer token provisioned via Quark-AI (`/api/auth/sdk/provision`).

---

*Incarna SDK — behavior_gate_v1 — 2026-05-10*
