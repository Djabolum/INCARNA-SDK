# Incarna — Pipeline Diagram

## Full behavioral pipeline

```
SCENE  ──────────────────────────────────────────────────────────────────────
│
│  affordances[]        world_state         body_state
│  (rest zones,         (heat, noise,        (energy, stress,
│   shade, human         safety, novelty)     comfort)
│   anchors...)
│
▼
ELYSIAN  ────────────────────────────────────────────────────────────────────
│
│  Transforms scene + body → meaning map
│
│  pull_vectors[]        avoid_vectors[]      dominant_axis
│  (what draws Orundra)  (what repels him)    (seek_restful_shelter…)
│
│  orientation_summary: { mode, posture }
│  interpretive_confidence: 0.84
│
│  If unavailable → fallback (semantic_layer.degraded = true)
│
▼
INCARNA RUNTIME  ────────────────────────────────────────────────────────────
│
│  homeostasis      → thermal_comfort, energy, stress_load, safety_feeling
│  appraisal        → valence, arousal, regulation_pressure
│  memory           → zone_biases[], preference_biases[]
│  orientation bias → reweights affordance scores
│  action policy    → selects action (move / dwell / approach / withdraw / idle)
│
│  ┌─────────────────────────────────────────────────────────────────┐
│  │  action decided here. nothing downstream changes this.          │
│  └─────────────────────────────────────────────────────────────────┘
│
▼
DRIFT CLASSIFIER (read-only)  ───────────────────────────────────────────────
│
│  reads: action, signals, continuity, internal, derived,
│         orientation, semantic_layer, drift_window (12 steps)
│
│  names the gap:
│
│  stable_embodiment   → no drift
│  semantic_degraded   → orientation unavailable
│  loop_collapse       → repeated action, no progression
│  agency_fade         → low-value motion, reduced initiative
│  player_mirroring    → role axis diluted by player
│  role_overstretch    → internal pressure above scene context
│  quest_drift         → objective alignment lost
│
│  output: drift_class + severity + confidence
│
▼
BEHAVIOR GATE MAPPER  ───────────────────────────────────────────────────────
│
│  drift_class + severity + runtime_policy → studio-safe gate
│
│  open        = step coherent
│  caution     = context flagged
│  restricted  = low confidence, severe drift
│
│  gate_source: "runtime_policy" | "drift_layer"
│
▼
STEP RESPONSE  ──────────────────────────────────────────────────────────────
│
│  ┌─────────────────────────────────────────────────────────────────┐
│  │  behavior_gate    "open" | "caution" | "restricted"             │
│  │  gate_layer       { gate_source, drift_gate, reason, version }  │
│  │  drift_layer      { drift_class, severity, confidence }         │
│  │  semantic_layer   { source, confidence, degraded }              │
│  │  action           { type, target, speed, dwell_time }           │
│  │  signals          { stability, fidelity_score, aggression… }    │
│  └─────────────────────────────────────────────────────────────────┘
│
├──── QPL TRACE (OBSERVE) ──────────────────────────────────────────────────▶  Génome
│     fire-and-forget → state-guard → state-core → Génome opérateur
│     payload: action_type, fidelity_score, drift_class, behavior_gate…
│
└──── STUDIO ────────────────────────────────────────────────────────────────
      reads behavior_gate → animates, gates dialogue, logs, triggers events
      does not see: homeostasis, appraisal, memory weights, fossil profile
```

---

## Layer responsibilities

```
Elysian           builds meaning map from scene
Incarna runtime   decides action (never overridden)
Drift classifier  names the behavioral gap (read-only)
Gate mapper       translates gap → studio signal
QPL               traces observation to Génome
Studio            authors the reaction
```

---

## The separation

```
observation  ≠  intervention
trace        ≠  decision
memory       ≠  structure
output       ≠  fossil
gate         ≠  command
```

The gate holds the retina, not the wheel.

---

## Compact version (for slide / README)

```
Scene
  └─ Elysian          (meaning map)
       └─ Incarna      (action decision ← never touched downstream)
            └─ Drift   (gap observation, read-only)
                 └─ Gate (open / caution / restricted → studio)
                      └─ QPL → Génome
```

---

*Incarna Runtime — behavior_gate_v1 — 2026-05-10*
