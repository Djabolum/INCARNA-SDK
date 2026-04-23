# Demo Scene

Suggested demo scene content:

- one NPC using `mode: remote`
- one NPC using `mode: local`
- one simple player input trigger
- on-screen labels for:
  - `state`
  - `stability`
  - `aggression`
  - `intent`

Recommended validation flow:

1. trigger a neutral prompt
2. trigger an aggressive prompt
3. unplug network and validate local fallback

Studio-ready quick test files now live in:

- `CognitiveSDK/Examples/StudioDemo/DemoController.cs`
- `CognitiveSDK/Examples/StudioDemo/NPCBehaviour.cs`
- `CognitiveSDK/Examples/StudioDemo/npc_profile.json`
