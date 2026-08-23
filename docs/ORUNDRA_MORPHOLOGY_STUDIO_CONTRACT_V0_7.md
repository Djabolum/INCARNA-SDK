# Orundra Morphology Studio Contract V0.7

## Status

```text
PUBLIC_BODY_STATE_FIELDS   = REMOVED
PUBLIC_MEMORY_FIELDS       = REMOVED
PUBLIC_EXPLAIN_FIELDS      = REMOVED
PUBLIC_QUARK_EVENT_FIELDS  = REMOVED
MORPHOLOGY_CANDIDATE       = OPTIONAL_GUARDED_VISUAL_ONLY
RUNTIME_ACTIVATION         = NONE
REAL_SOURCE_READ           = NONE
```

V0.7 aligns the Unity-facing data model with the actual studio contract. The
server may use homeostasis, appraisal, memory, and Elysian internally, but these
process inputs are not public outputs.

## Migration from the legacy DTO

| Removed public field family | Studio-safe replacement |
| --- | --- |
| homeostasis and appraisal fields | aggregate `pressure`, `control`, `stability` |
| `dominant_need`, `dominant_pull`, `dominant_risk` | `gate_layer.studio_safe_reason` and `drift_layer` |
| zone and reward memory weights | no public replacement |
| `quark_event` | no public replacement |
| inspect `zone_memory` | compact orientation and Elysian resolution only |

The absence of a replacement is deliberate where a replacement would recreate
the private process under a different name.

## Admitted response surface

The public result keeps gameplay-safe action, signals, continuity, identity,
semantic, drift, gate, and presence projection fields. It may expose:

```text
morphology_candidate
morphology_projection
```

only when the projection receipt is exact and has `status=produced`.

The SDK also exposes `morphology_candidate_admitted`. This local boolean keeps
the source process receipt distinct from the SDK output decision: a producer
may report `produced` while the public candidate is still rejected locally.

## Fail-closed candidate checks

- exact schema `orundra_morphology_candidate_v0`;
- exact source `elysian.morphing`;
- exact entity `orundra`;
- expected session and entity when the request supplies them;
- non-empty trace identity;
- `session_only` persistence;
- known phase, regions, and effects;
- all signals and strengths in `[0,1]`;
- at most 16 zones and 16 effects;
- bounded non-empty reasons;
- all authority bits false;
- `canon_promotion_allowed=false`.

## Invariants

```text
output != process
memory != structure
source-side body mapping != public body state
candidate != body edit
projection receipt != activation authority
visual session layer != stable memory
```

The SDK does not enable the relay, call Elysian directly, deploy a runtime, or
promote any visual candidate to canon.
