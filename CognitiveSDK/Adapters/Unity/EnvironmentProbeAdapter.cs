using UnityEngine;
using CognitiveSDK.Runtime;
using System;
using System.Collections.Generic;

public class EnvironmentProbeAdapter : MonoBehaviour
{
    [Serializable]
    private class SceneAffordanceAnchor
    {
        public string id;
        public string type;
        public string[] aliases;
        public Transform transform;
        public float quality;
        public float familiarity;
    }

    [Serializable]
    private class SceneScalarProbe
    {
        public string kind;
        public string[] aliases;
        public Transform transform;
        public float radius;
        public float intensity;
    }

    [Header("Scene World Defaults")]
    public string zoneId = "unity_demo_scene";
    [Range(0f, 1f)] public float timeOfDay = 0.63f;
    [Range(10f, 35f)] public float ambientTemp = 23.5f;
    [Range(0f, 1f)] public float baseNoiseLevel = 0.12f;
    [Range(0f, 1f)] public float baseSafetyIndex = 0.76f;
    [Range(0f, 1f)] public float baseNoveltyIndex = 0.34f;

    private readonly List<SceneAffordanceAnchor> anchors = new List<SceneAffordanceAnchor>();
    private readonly List<SceneScalarProbe> probes = new List<SceneScalarProbe>();
    private float nextRefreshTime;

    public float GetGateDamp(CognitiveResult result)
    {
        if (result == null || string.IsNullOrWhiteSpace(result.behavior_gate))
        {
            return 1f;
        }

        if (result.behavior_gate == "restricted") return 0.35f;
        if (result.behavior_gate == "caution") return 0.72f;
        return 1f;
    }

    public float GetDestinationBias(CognitiveResult result)
    {
        var hint = result?.destination_hint ?? "hold_position";

        if (hint == "sun_patch") return 0.85f;
        if (hint == "shade_zone") return -0.75f;
        if (hint == "rest_zone") return -0.2f;
        if (hint == "human_anchor") return 0.45f;
        if (hint == "novelty_point") return 0.6f;
        return 0f;
    }

    public WorldStateDto BuildWorldState(Vector3 origin)
    {
        RefreshAnchorsIfNeeded();

        float shadeDistance = GetDistanceToType(origin, "shade_zone");
        float restDistance = GetDistanceToType(origin, "rest_zone");
        float humanDistance = GetDistanceToType(origin, "human_anchor");
        float sunDistance = GetDistanceToType(origin, "sun_patch");
        float heatSignal = SampleProbeSignal(origin, "heat");
        float coolSignal = SampleProbeSignal(origin, "cool");
        float noiseSignal = SampleProbeSignal(origin, "noise");
        float safetySignal = SampleProbeSignal(origin, "safety");
        float noveltySignal = SampleProbeSignal(origin, "novelty");

        float solarExposure = Mathf.Clamp01(
            0.55f
            + DistanceInfluence(sunDistance, 4.5f, 0.28f)
            - DistanceInfluence(shadeDistance, 4f, 0.34f)
            + heatSignal * 0.12f
            - coolSignal * 0.16f
        );

        float safetyIndex = Mathf.Clamp01(
            baseSafetyIndex
            + DistanceInfluence(shadeDistance, 4.2f, 0.1f)
            + DistanceInfluence(humanDistance, 5.2f, 0.06f)
            - DistanceInfluence(sunDistance, 2.2f, 0.08f)
            + safetySignal * 0.22f
            - noiseSignal * 0.08f
        );

        float noveltyIndex = Mathf.Clamp01(
            baseNoveltyIndex
            + DistanceInfluence(GetDistanceToType(origin, "novelty_point"), 4f, 0.22f)
            + noveltySignal * 0.28f
        );

        return new WorldStateDto
        {
            zone_id = zoneId,
            time_of_day = timeOfDay,
            solar_exposure = solarExposure,
            ambient_temp = ambientTemp + heatSignal * 6f - coolSignal * 4.5f,
            shade_distance = ToDtoDistance(shadeDistance),
            rest_zone_distance = ToDtoDistance(restDistance),
            human_distance = ToDtoDistance(humanDistance),
            human_familiarity = GetAnchorFamiliarity("human_anchor"),
            noise_level = Mathf.Clamp01(baseNoiseLevel + DistanceInfluence(humanDistance, 3f, 0.08f) + noiseSignal * 0.35f),
            safety_index = safetyIndex,
            novelty_index = noveltyIndex,
        };
    }

    public AffordanceDto[] BuildAffordances(Vector3 origin)
    {
        RefreshAnchorsIfNeeded();

        var affordances = new List<AffordanceDto>(anchors.Count);
        foreach (var anchor in anchors)
        {
            if (anchor.transform == null)
            {
                continue;
            }

            affordances.Add(new AffordanceDto
            {
                id = anchor.id,
                type = anchor.type,
                distance = Vector3.Distance(origin, anchor.transform.position),
                quality = anchor.quality,
            });
        }

        return affordances.ToArray();
    }

    public Transform ResolveTargetTransform(CognitiveResult result, Vector3 origin)
    {
        RefreshAnchorsIfNeeded();

        if (result == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(result.target_affordance_id))
        {
            for (int i = 0; i < anchors.Count; i++)
            {
                if (string.Equals(anchors[i].id, result.target_affordance_id, StringComparison.OrdinalIgnoreCase))
                {
                    return anchors[i].transform;
                }
            }
        }

        string preferredType = GetPreferredTargetType(result);
        SceneAffordanceAnchor best = null;
        float bestScore = float.NegativeInfinity;

        for (int i = 0; i < anchors.Count; i++)
        {
            var anchor = anchors[i];
            if (anchor.transform == null || anchor.type != preferredType)
            {
                continue;
            }

            float distance = Vector3.Distance(origin, anchor.transform.position);
            float score = anchor.quality - distance * 0.08f;
            if (score > bestScore)
            {
                best = anchor;
                bestScore = score;
            }
        }

        return best != null ? best.transform : null;
    }

    public string DescribeTarget(CognitiveResult result, Vector3 origin)
    {
        var target = ResolveTargetTransform(result, origin);
        return target != null ? target.name : "hold_position";
    }

    private void RefreshAnchorsIfNeeded()
    {
        if (Time.unscaledTime < nextRefreshTime && anchors.Count > 0)
        {
            return;
        }

        anchors.Clear();
        RegisterAnchor("sun_patch_A", "sun_patch", 0.74f, 0.15f, "SunAnchor", "SunPatch", "Sun");
        RegisterAnchor("shade_zone_A", "shade_zone", 0.82f, 0.25f, "ShadeAnchor", "ShadeZone", "Shade");
        RegisterAnchor("rest_zone_A", "rest_zone", 0.77f, 0.3f, "RestAnchor", "RestZone", "Rest");
        RegisterAnchor("human_anchor_A", "human_anchor", 0.71f, 0.62f, "HumanAnchor", "HumanZone", "Human");
        RegisterAnchor("novelty_point_A", "novelty_point", 0.65f, 0.1f, "NoveltyAnchor", "NoveltyPoint", "Novelty");
        probes.Clear();
        RegisterProbe("heat", 4.2f, 0.82f, "HeatProbe", "HeatZone");
        RegisterProbe("cool", 4.8f, 0.76f, "CoolProbe", "CoolZone");
        RegisterProbe("noise", 3.2f, 0.65f, "NoiseProbe", "NoiseZone");
        RegisterProbe("safety", 5.4f, 0.88f, "SafetyProbe", "SafeZone");
        RegisterProbe("novelty", 4.6f, 0.72f, "NoveltyProbe", "NoveltyZone");
        nextRefreshTime = Time.unscaledTime + 0.5f;
    }

    private void RegisterAnchor(string id, string type, float quality, float familiarity, params string[] aliases)
    {
        var anchorTransform = FindAnchorTransform(aliases);
        if (anchorTransform == null)
        {
            return;
        }

        anchors.Add(new SceneAffordanceAnchor
        {
            id = id,
            type = type,
            aliases = aliases,
            transform = anchorTransform,
            quality = quality,
            familiarity = familiarity,
        });
    }

    private void RegisterProbe(string kind, float radius, float intensity, params string[] aliases)
    {
        var probeTransform = FindAnchorTransform(aliases);
        if (probeTransform == null)
        {
            return;
        }

        probes.Add(new SceneScalarProbe
        {
            kind = kind,
            aliases = aliases,
            transform = probeTransform,
            radius = radius,
            intensity = intensity,
        });
    }

    private Transform FindAnchorTransform(string[] aliases)
    {
        var sceneTransforms = GameObject.FindObjectsOfType<Transform>();
        for (int i = 0; i < sceneTransforms.Length; i++)
        {
            var candidate = sceneTransforms[i];
            if (candidate == null)
            {
                continue;
            }

            for (int j = 0; j < aliases.Length; j++)
            {
                if (string.Equals(candidate.name, aliases[j], StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    private float GetDistanceToType(Vector3 origin, string type)
    {
        float best = float.PositiveInfinity;

        for (int i = 0; i < anchors.Count; i++)
        {
            if (anchors[i].type != type || anchors[i].transform == null)
            {
                continue;
            }

            float distance = Vector3.Distance(origin, anchors[i].transform.position);
            if (distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    private float SampleProbeSignal(Vector3 origin, string kind)
    {
        float total = 0f;
        float weight = 0f;

        for (int i = 0; i < probes.Count; i++)
        {
            var probe = probes[i];
            if (probe.kind != kind || probe.transform == null)
            {
                continue;
            }

            float distance = Vector3.Distance(origin, probe.transform.position);
            float local = 1f - Mathf.Clamp01(distance / Mathf.Max(0.01f, probe.radius));
            if (local <= 0f)
            {
                continue;
            }

            total += local * probe.intensity;
            weight += 1f;
        }

        return weight > 0f ? Mathf.Clamp01(total / weight) : 0f;
    }

    private float GetAnchorFamiliarity(string type)
    {
        for (int i = 0; i < anchors.Count; i++)
        {
            if (anchors[i].type == type)
            {
                return anchors[i].familiarity;
            }
        }

        return 0f;
    }

    private string GetPreferredTargetType(CognitiveResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.destination_hint))
        {
            return result.destination_hint;
        }

        if (result.action_type == "withdraw")
        {
            return "shade_zone";
        }

        if (result.action_type == "approach")
        {
            return "human_anchor";
        }

        return "rest_zone";
    }

    private float DistanceInfluence(float distance, float maxDistance, float weight)
    {
        if (float.IsPositiveInfinity(distance))
        {
            return 0f;
        }

        return Mathf.Clamp01(1f - (distance / Mathf.Max(0.01f, maxDistance))) * weight;
    }

    private float ToDtoDistance(float distance)
    {
        return float.IsPositiveInfinity(distance) ? 999f : distance;
    }
}
