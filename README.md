# Incarna SDK Public

Public studio-safe surface for the Incarna game integration stack.

This repository is intentionally limited to:

- `CognitiveSDK/`
- Unity-facing runtime contracts
- scene-facing adapters
- demo-safe examples
- documentation that can be shown externally

This repository intentionally excludes:

- the embodied runtime core
- server routes
- private thresholds and policies
- confidential release artifacts
- internal governance material

If you need access beyond this surface, that is a separate private review path and does not travel with the public SDK.

The current studio contract is V0.7. It removes misleading public internal-state
DTOs and admits the optional Orundra morphology candidate only through an exact,
visual-only, fail-closed guard. See
`docs/ORUNDRA_MORPHOLOGY_STUDIO_CONTRACT_V0_7.md`.
