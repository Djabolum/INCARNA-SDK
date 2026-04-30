# Cognitive Behavior SDK

Inject dynamic cognitive behavior into your NPCs.

## Overview

Cognitive Behavior SDK helps game studios build NPCs that react dynamically without dialogue trees or hardcoded behavior graphs.

Tagline:

`Stop scripting NPCs. Start injecting cognition.`

Public companion surface:

- `https://game-ai.cordee.ovh`
- bilingual `FR/EN` shell with browser-locale auto-detection
- manual locale switch persisted locally for studio demos and public discovery

From the studio point of view, the API stays simple:

```csharp
var result = npc.Step(playerInput);
```

The studio does not need to understand the cognitive core behind the system.
It only consumes:

- state
- stability
- aggression
- intent

## Product Positioning

What studios buy:

- dynamic NPC behavior
- adaptive reactions
- stable runtime abstraction
- optional remote cognition backend

What they do not need to learn:

- Quarks
- Fossils
- Transcendeur internals

What makes it different:

- no dialogue trees
- no runtime LLM dependency in the client
- no prompts exposed to the studio
- behavior exposed as gameplay-ready signals

## Core Model

Internal stack:

- `Fossil` -> static structure
- `Quark` -> dynamic behavior transitions
- `SDK` -> runtime abstraction for games

Studio-facing abstraction:

- `NPC Profile` -> configuration file
- `CognitiveNPC` -> runtime object
- `CognitiveResult` -> gameplay-ready result

## Architecture

```text
Game Engine (Unity / Unreal)
        |
        v
Cognitive Behavior SDK
        |
        v
Bridge API (optional)
        |
        v
External cognitive backend
```

## Folder Layout

```text
CognitiveSDK/
├── Runtime/
│   ├── ActionExecutor.cs
│   ├── CognitiveNPC.cs
│   ├── CognitiveState.cs
│   ├── CognitiveEngine.cs
│   └── CognitiveTypes.cs
├── Adapters/
│   ├── HttpBridgeClient.cs
│   └── LocalMockEngine.cs
│   └── Unity/
│       ├── NavMeshMoveAdapter.cs
│       ├── AnimatorAdapter.cs
│       └── EnvironmentProbeAdapter.cs
├── Config/
│   ├── NPCProfile.json
│   └── SDKConfig.asset
├── Editor/
│   └── NPCInspector.cs
└── Examples/
    ├── DemoScene.md
    └── StudioDemo/
        ├── DemoController.cs
        ├── DemoHUD.cs
        ├── NPCBehaviour.cs
        ├── SCENE_SETUP.md
        └── npc_profile.json
```

## Pitch-ready material

The SDK now includes bilingual studio-facing assets:

- `STUDIO_DEMO_SCRIPT.md`
- `STUDIO_PITCH_FR.md`
- `STUDIO_PITCH_EN.md`
- `STUDIO_ORAL_DEMO_FR.md`
- `STUDIO_ORAL_DEMO_EN.md`

These are meant to support short live demos, first calls with studios, and internal alignment before packaging a Unity sample.

## Unity Demo Scene

If you want to learn the product by manipulating it instead of memorizing it, start with:

- `Examples/StudioDemo/DemoController.cs`
- `Examples/StudioDemo/DemoHUD.cs`
- `Examples/StudioDemo/NPCBehaviour.cs`
- `Runtime/ActionExecutor.cs`
- `Examples/StudioDemo/SCENE_SETUP.md`

This gives you a minimal scene where one NPC reacts to two player inputs and exposes runtime safety directly in the editor.

For a minimal token + remote-call validation path, use:

- `Examples/StudioDemo/SDKSmokeTest.cs`

The scene now also gives you a visible loop for studio demos:

- `OPEN` -> stable pulse, direct runtime
- `CAUTION` -> warmer pulse, monitored drift
- `RESTRICTED` -> softened behavior, visible instability

The embodied path now includes a first Reality Adapter layer:

- `ActionExecutor` translates `move / dwell / idle / approach / withdraw`
- `NPCBehaviour` stays focused on gate reading, confidence pulse, and visual state
- Unity consumes a stable action contract instead of the raw internal cognition
- `NavMeshMoveAdapter`, `AnimatorAdapter`, and `EnvironmentProbeAdapter` are now split out as separate motor-facing responsibilities

The scene-facing path is now more concrete:

- affordance anchors (`SunAnchor`, `ShadeAnchor`, `RestAnchor`, `HumanAnchor`) are resolved as real target destinations
- environment probes (`HeatProbe`, `CoolProbe`, `NoiseProbe`, `SafetyProbe`, `NoveltyProbe`) modulate the embodied world state instead of acting as direct targets
- `DemoController` builds `WorldStateDto` and `AffordanceDto[]` from the scene at runtime instead of using only hardcoded demo values
- `ActionExecutor` can now move and orient the NPC toward resolved scene targets while preserving the studio-safe contract

## Installation (Unity)

1. Import the `CognitiveSDK` folder into your Unity project.
2. Add your NPC profile JSON to the project.
3. Optionally configure a bridge endpoint for remote behavior.
4. Provision an SDK token and place it in `sdk_token` for remote access.
5. Instantiate a `CognitiveNPC` in your scene scripts.

## Quick Start

```csharp
using CognitiveSDK.Runtime;

public class NPCDriver : UnityEngine.MonoBehaviour
{
    private CognitiveNPC npc;

    void Start()
    {
        var profile = CognitiveProfile.LoadFromJson(Resources.Load<TextAsset>("NPCProfile").text);
        npc = new CognitiveNPC(profile);
    }

    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var result = npc.Step("I challenge you");
            UnityEngine.Debug.Log(result.state);
        }
    }
}
```

## Full Unity Example

```csharp
using CognitiveSDK.Runtime;
using UnityEngine;

public class SDKChallengeExample : MonoBehaviour
{
    private CognitiveNPC npc;

    void Start()
    {
        var profile = CognitiveProfile.LoadFromJson(Resources.Load<TextAsset>("npc_profile").text);
        profile.sdk_token = "REPLACE_WITH_SDK_TOKEN";

        npc = new CognitiveNPC(profile);

        var result = npc.Step("Je te défie");

        Debug.Log($"State: {result.state}");
        Debug.Log($"Intent: {result.intent}");
        Debug.Log($"Gate: {result.behavior_gate}");
        Debug.Log($"Policy: {result.runtime_policy}");
        Debug.Log($"Stability: {result.stability}");
        Debug.Log($"Aggression: {result.aggression}");
        Debug.Log($"Pressure: {result.pressure}");
        Debug.Log($"Control: {result.control}");
        Debug.Log($"Confidence: {result.behavior_confidence}");
        Debug.Log($"Fidelity: {result.fidelity_score}");
        Debug.Log($"Continuity: {result.continuity}");
        Debug.Log($"Action: {result.action_type} -> {result.target_affordance_id}");

        switch (result.behavior_gate)
        {
            case "restricted":
                Debug.Log("NPC should soften behavior or hold position.");
                break;
            case "caution":
                Debug.Log("NPC can act, but the runtime should stay monitored.");
                break;
            default:
                Debug.Log("NPC can use the full expressive range.");
                break;
        }
    }
}
```

## Unity Smoke Test

Attach `Examples/StudioDemo/SDKSmokeTest.cs` to any GameObject, assign `npc_profile.json`, then either:

- enable `runOnStart`
- or use the inspector context menu `Run SDK Smoke Test`

The component logs a compact remote runtime trace:

- `state`
- `behavior_gate`
- `runtime_policy`
- `stability / aggression / pressure / control`
- `fidelity_score / behavior_confidence / alignment_state`
- `intent`
- `text`

This is the fastest way to validate that:

- the `sdk_token` is accepted
- `npc.Step(...)` reaches the remote bridge
- the studio-facing text stays free of internal route labels

## Core API

### CognitiveNPC

```csharp
var npc = new CognitiveNPC(profile);
var result = npc.Step(playerInput);
```

### CognitiveResult

```csharp
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
}
```

## Live Endpoint

Production runtime endpoint:

```text
POST https://game-ai.cordee.ovh/api/v1/sdk/step
```

Reset dev session:

```text
POST https://game-ai.cordee.ovh/api/v1/sdk/reset-session
```

Example request:

```json
{
  "input": "I challenge you",
  "profile": {
    "fossil_id": "fossil_valexa_01",
    "quark_id": "quark_valexa_01",
    "mode": "remote",
    "npc_name": "Valexa"
  }
}
```

SDK token provisioning:

```text
POST https://quark-ai.cordee.ovh/api/auth/sdk/provision
Authorization: Bearer <quark_access_token>
```

Requirements:

- the authenticated Quark account must have the `game_adapter` add-on enabled
- the returned token is meant for the SDK surface and should be stored in `profile.sdk_token`

Example provisioning response:

```json
{
  "token": "eyJ...",
  "jti": "a1b2c3...",
  "label": "unity-studio-guard",
  "expires_at": "2026-04-24T12:00:00+00:00",
  "ttl_hours": 24,
  "bridge_url": "https://game-ai.cordee.ovh/api/v1/sdk/step",
  "usage": {
    "header": "Authorization: Bearer eyJ...",
    "unity_profile_field": "sdk_token"
  }
}
```

Provisioning with `curl`:

```bash
curl -X POST https://quark-ai.cordee.ovh/api/auth/sdk/provision \
  -H "Authorization: Bearer <quark_access_token>" \
  -H "Content-Type: application/json" \
  -d '{"label":"unity-studio-guard","ttl_hours":24}'
```

Example response:

```json
{
  "state": "aggressive",
  "stability": 0.42,
  "aggression": 0.81,
  "pressure": 0.77,
  "control": 0.31,
  "fidelity_score": 0.84,
  "behavior_confidence": 0.73,
  "alignment_state": "aligned",
  "behavior_gate": "open",
  "runtime_policy": "direct_runtime",
  "intent": "reponse",
  "text": "..."
}
```

Observed live example for `npc.Step("Je te défie")` in remote mode on `2026-04-23`:

```json
{
  "state": "analytical",
  "stability": 0.35,
  "aggression": 0.12,
  "pressure": 0.321,
  "control": 0.498,
  "fidelity_score": 0.226,
  "behavior_confidence": 0.266,
  "alignment_state": "drifting",
  "behavior_gate": "restricted",
  "runtime_policy": "fallback_to_safe_behavior",
  "intent": "reponse",
  "continuity": 0,
  "text": "Le défi que tu proposes soulève une tension sous-jacente entre l'aspiration à l'exploration et la structure nécessaire à cette exploration. En posant cette question, on peut se demander ce qui en nous cherche à se confronter, à se mesurer à une autre perspective. Ce besoin d'interaction peut également masquer une peur de la stagnation ou de l'immobilisme. En éclairant cette dynamique, nous pourrions dévoiler les fondations de notre échange. Quels aspects souhaites-tu vraiment aborder dans cette confrontation?"
}
```

Live values can vary slightly from one session to another, but the SDK response should stay studio-facing and must not expose internal route labels such as `sdk_npc_runtime`.

## Modes

### Remote Mode

Recommended for production:

- uses the bridge backend
- dynamic live behavior
- external cognitive orchestration
- server-side short memory per NPC session
- supports dedicated SDK bearer tokens in `profile.sdk_token`
- forwards the request through a protected service bridge without exposing internal server secrets to the client
- exposes alignment and behavior confidence without exposing Quark internals

In `local` mode, `Step("Je te défie")` may stay neutral because the mock engine reacts only to a small keyword set.
In `remote` mode, the same call returns a live `CognitiveResult` driven by the server runtime and session context.

### Local Mode

Useful for fallback and prototyping:

- deterministic transitions
- no remote dependency
- simplified runtime behavior

## Runtime Safety Layer

The SDK now exposes a studio-safe fidelity layer:

- `fidelity_score`
- `alignment_state`
- `behavior_confidence`
- `behavior_gate`

Studios still do not see Quark or Transcendeur internals.
They only receive stable runtime signals to decide whether behavior should stay open, be monitored, or be softened.

## Gameplay Mapping

The SDK does not control gameplay.

Studios keep control of the game loop:

```csharp
switch (result.state)
{
    case "aggressive":
        Attack();
        break;
    case "defensive":
        Defend();
        break;
    case "unstable":
        Flee();
        break;
}
```

## Performance

- Local mode: sub-millisecond scale for lightweight transitions
- Remote mode: depends on bridge latency
- No GPU required inside the client SDK

## Security

- cognitive core remains server-side
- studios consume runtime behavior, not internals
- local mode remains available if network is unavailable
- Quark / Fossil / Transcendeur do not need to be exposed in studio UX

## Roadmap

- Unreal plugin
- visual NPC inspector
- behavior debugger
- marketplace integration

## Inspect Surface

The studio inspect route can now expose two levels of reading:

- a compact orientation summary:
  - `dominant_axis`
  - `mode`
  - `posture`
- an optional `Elysian` debug resolution:
  - primary zone
  - salience gap
  - interpretation notes

This remains a reading surface only.
It does not turn `Elysian` into an action planner.

## Studio Demo

A ready-to-wire Unity demo scaffold is included in:

- `CognitiveSDK/Examples/StudioDemo/`

It is designed to let a studio test the runtime in under five minutes.

## Contact

`ai-zi-me@cordee.ovh`
