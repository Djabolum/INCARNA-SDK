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
4. Instantiate a `CognitiveNPC` in your scene scripts.

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

## Modes

### Remote Mode

Recommended for production:

- uses the bridge backend
- dynamic live behavior
- external cognitive orchestration
- server-side short memory per NPC session
- exposes alignment and behavior confidence without exposing Quark internals

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

## Studio Demo

A ready-to-wire Unity demo scaffold is included in:

- `CognitiveSDK/Examples/StudioDemo/`

It is designed to let a studio test the runtime in under five minutes.

## Contact

`ai-zi-me@cordee.ovh`
