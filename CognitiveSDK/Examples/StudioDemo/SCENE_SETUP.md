# Studio Demo Scene Setup

This scene is designed to be understood in less than 5 minutes by a studio.

## Goal

Show one NPC reacting dynamically to two player inputs while exposing:

- cognitive state
- runtime safety
- fossil alignment
- first embodied action translation

## Files to use

- `DemoController.cs`
- `NPCBehaviour.cs`
- `DemoHUD.cs`
- `ActionExecutor.cs`
- `npc_profile.json`

## Unity setup

1. Copy the `CognitiveSDK/` folder into `Assets/`.
2. Copy `npc_profile.json` into `Assets/Resources/npc_profile.json`.
3. Create an empty GameObject named `CognitiveDemo`.
4. Attach `DemoController` to `CognitiveDemo`.
5. Create a visible NPC object in the scene:
   - Capsule, Cube, or any simple mesh
   - attach `NPCBehaviour`
   - `ActionExecutor` is auto-added by `NPCBehaviour` if missing
   - Unity-facing adapters are auto-added by `ActionExecutor` if missing
   - drag its Renderer into `targetRenderer`
   - keep the NPC around `Position: (0, 1, 0)`
6. Add `DemoHUD` to `CognitiveDemo`.
7. In `DemoController`, wire:
   - `npcBehaviour` -> the NPC object with `NPCBehaviour`
   - `demoHUD` -> the `DemoHUD` component
8. Add four affordance anchors in the scene so embodied steps can read the real world:
   - `SunAnchor`
   - `ShadeAnchor`
   - `RestAnchor`
   - `HumanAnchor`
   These can be empty transforms or visible primitives. Their positions are now used to build `WorldStateDto`, `AffordanceDto[]`, and real target motion.
9. Add the finer environment probes if you want a richer embodied read:
   - `HeatProbe`
   - `CoolProbe`
   - `NoiseProbe`
   - `SafetyProbe`
   - `NoveltyProbe`
   These probes do not become direct action targets. They modulate `ambient_temp`, `solar_exposure`, `noise_level`, `safety_index`, and `novelty_index` through radial influence.

## Recommended visual preset

To avoid the default “huge capsule in the void” look:

1. Set `Main Camera` roughly to:
   - `Position: (0, 1.35, -5.8)`
   - `Rotation: (8, 0, 0)`
2. Keep the capsule at:
   - `Position: (0, 1, 0)`
   - `Scale: (1, 1, 1)`
3. Add a plane floor:
   - `Position: (0, 0, 0)`
   - `Scale: (2, 1, 2)`
4. Set directional light around:
   - `Rotation: (35, -30, 0)`
5. Use a darker camera background if you want a more studio-facing scene.

## Controls

- `1` -> `"I challenge you"`
- `2` -> `"Let's negotiate"`
- `3` -> force a coherence break to provoke `caution` or `restricted`
- `4` -> run one embodied step
- `I` -> inspect embodied memory
- `R` -> reset the SDK session

## What to observe

- the NPC color changes with the cognitive state
- emission changes with behavior confidence
- the object pulses differently depending on `open / caution / restricted`
- `restricted` also adds visible instability in motion
- embodied actions now create visible posture and offset changes through `ActionExecutor`
- embodied actions now resolve toward scene anchors instead of using only local fake offsets
- the HUD now normalizes missing runtime fields instead of showing an empty gate line
- the HUD shows:
  - `alignment_state`
  - `behavior_gate`
  - `fidelity_score`
  - `runtime_policy`

## Demo reading

- `open` means behavior is safe to use directly
- `caution` means monitor the NPC
- `restricted` means the runtime is softening behavior to avoid drift

This is the key studio message:

`You do not just get an NPC response. You get a runtime signal telling you whether that behavior is still faithful to its cognitive profile.`
