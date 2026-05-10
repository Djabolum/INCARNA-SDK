# What Studios Can Do With Incarna

Incarna is a behavioral runtime for NPCs.
It produces one observable step at a time — action, internal state, and a confidence signal.

This document shows what studios can build on top of that.

---

## The core integration

One call per NPC tick:

```csharp
var result = npc.StepEmbodied(request);
```

One signal to integrate:

```csharp
result.behavior_gate  // "open" | "caution" | "restricted"
```

That signal reflects whether the NPC's current behavior is coherent,
flagged, or degraded. The studio decides what to do with it.

---

## What studios can build

### Debug overlay (development)

Display gate state, drift class, and semantic confidence during playtesting.
Flag steps that need tuning before shipping.

```
Gate:     CAUTION  (drift_layer)
Drift:    role_overstretch  [moderate]
Semantic: elysian  0.84  degraded: false
Reason:   role intensity rising above scene context
```

See: `Source/Neoxys/Debug/OrundraDebugHUD` for a ready UE5 implementation.

### Adaptive animation blend

```csharp
float blendSpeed = result.behavior_gate == "restricted" ? 0.3f : 1.0f;
animator.SetFloat("BlendSpeed", blendSpeed);
```

Gate `restricted` → slow down expressive animation.
Gate `open` → full expression.

### Dialogue gating

```csharp
if (result.behavior_gate == "restricted")
{
    // Avoid escalating dialogue branches.
    // Route to neutral or observational responses only.
    dialogueManager.SetMode(DialogueMode.Safe);
}
```

### Narrative event triggers

```csharp
if (result.drift_layer_drift_class == "player_mirroring"
    && result.drift_layer_severity == "moderate")
{
    // The NPC is losing its own axis.
    // Trigger a narrative beat: resistance, silence, withdrawal.
    narrativeEngine.TriggerEvent("npc_autonomy_friction");
}
```

### Analytics and session logging

```csharp
analyticsService.Log(new NPCStep {
    SessionId    = result.session_id,
    Gate         = result.behavior_gate,
    DriftClass   = result.drift_layer_drift_class,
    Severity     = result.drift_layer_severity,
    FidelityScore = result.fidelity_score,
    Timestamp    = DateTime.UtcNow,
});
```

Use this to identify scenes where NPCs drift consistently.
Tune affordance placement or profile parameters accordingly.

### Graceful degradation

```csharp
if (result.semantic_layer_degraded)
{
    // Orientation data unavailable — NPC acts on internal state only.
    // Studio may choose to show a subtle visual indicator
    // or simply let the NPC continue without exposing the technical condition.
}
```

### Behavior gate history

Track gate transitions over a session to detect patterns:

```csharp
gateHistory.Add(result.behavior_gate);

var cautionStreak = gateHistory.TakeLast(10).Count(g => g != "open");
if (cautionStreak >= 8)
{
    // NPC has been under pressure for 8+ consecutive steps.
    // Scene tension is sustained — consider a narrative release or reset.
}
```

---

## What studios do NOT need to do

- Parse homeostasis values
- Understand arousal or regulation_pressure
- Manage semantic orientation layers
- Tune fossil profiles to get basic behavior
- Implement your own drift detection

Incarna handles the interior. The studio authors the reaction.

---

## The gate contract

```
open       — step is coherent. Full integration.
caution    — context flagged. Log, slow, monitor.
restricted — low confidence. Safe fallback.

behavior_gate is advisory only.
It does not alter the NPC's action.
The gate holds the retina, not the wheel.
```

---

## Sandbox

```
POST https://game-ai.cordee.ovh/api/v1/sdk/step
Authorization: Bearer {sdk_token}
```

Token provisioning: `https://quark-ai.cordee.ovh/api/auth/sdk/provision`

Contact: `ai-zi-me@cordee.ovh`

---

*Incarna SDK — behavior_gate_v1 — 2026-05-10*
