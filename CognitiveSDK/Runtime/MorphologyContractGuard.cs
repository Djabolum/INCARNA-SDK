using System;

namespace CognitiveSDK.Runtime
{
    public static class MorphologyContractGuard
    {
        private static readonly string[] Phases =
        {
            "cosmic_tear", "condensation", "living_neoxys_body"
        };

        private static readonly string[] Regions =
        {
            "inner_void_surface", "edge_contour", "shoulders", "hands", "face",
            "thorax", "back", "post_auricular", "eyes", "halo", "limbs"
        };

        private static readonly string[] Effects =
        {
            "inner_universe_visibility", "warm_nebula_bloom",
            "anthracite_skin_condensation", "nano_alloy_density",
            "trifocal_pupil_hint", "thermal_groove_emergence",
            "cosmic_edge_instability", "controlled_fragmentation",
            "shadow_veil_anchor", "refuge_density_growth"
        };

        public static bool IsSafeCandidate(
            OrundraMorphologyCandidateDto candidate,
            string expectedSessionId = null,
            string expectedEntityId = null)
        {
            if (candidate == null ||
                candidate.schema != "orundra_morphology_candidate_v0" ||
                string.IsNullOrWhiteSpace(candidate.session_id) ||
                string.IsNullOrWhiteSpace(candidate.trace_id) ||
                candidate.source != "elysian.morphing" ||
                candidate.entity_id != "orundra" ||
                candidate.persistence != "session_only" ||
                !Contains(Phases, candidate.phase) ||
                candidate.authority == null ||
                candidate.authority.runtime_body_change ||
                candidate.authority.stable_body_change ||
                candidate.authority.memory_write ||
                candidate.authority.promotion_to_canon ||
                candidate.signals == null ||
                !candidate.fossilizable ||
                candidate.canon_promotion_allowed)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(expectedSessionId) && candidate.session_id != expectedSessionId)
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(expectedEntityId) && candidate.entity_id != expectedEntityId)
            {
                return false;
            }

            var signals = candidate.signals;
            if (!Unit(signals.cosmic_void_phase) ||
                !Unit(signals.body_formation_progress) ||
                !Unit(signals.trust_construction_progress) ||
                !Unit(signals.shelter_density) ||
                !Unit(signals.warmth_exposure) ||
                !Unit(signals.shadow_affinity) ||
                !Unit(signals.chaos_pressure) ||
                !Unit(signals.novelty_pressure) ||
                !Unit(signals.neoxys_density))
            {
                return false;
            }

            if (candidate.materialization_zones == null || candidate.materialization_zones.Length > 16 ||
                candidate.candidate_effects == null || candidate.candidate_effects.Length > 16)
            {
                return false;
            }

            foreach (var zone in candidate.materialization_zones)
            {
                if (zone == null || !Contains(Regions, zone.region) || !Unit(zone.strength) || !Reason(zone.reason))
                {
                    return false;
                }
            }

            foreach (var effect in candidate.candidate_effects)
            {
                if (effect == null || !Contains(Regions, effect.region) || !Contains(Effects, effect.effect) ||
                    !Unit(effect.strength) || !Reason(effect.reason))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSafeProjection(MorphologyProjectionReceiptDto projection)
        {
            return projection != null &&
                projection.schema == "orundra_morphology_projection_receipt_v0" &&
                Contains(new[] { "disabled", "produced", "degraded", "rejected" }, projection.status) &&
                projection.source == "incarna.internal_to_elysian" &&
                projection.mode == "post_decision_visual_only" &&
                !projection.body_state_exposed &&
                !projection.influences_action &&
                !projection.behavior_override &&
                !projection.stable_memory_write &&
                !projection.canon_promotion_allowed;
        }

        private static bool Unit(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;
        }

        private static bool Reason(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= 160;
        }

        private static bool Contains(string[] allowed, string value)
        {
            if (value == null) return false;
            foreach (var item in allowed)
            {
                if (string.Equals(item, value, StringComparison.Ordinal)) return true;
            }
            return false;
        }
    }
}
