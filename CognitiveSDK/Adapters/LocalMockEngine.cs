using CognitiveSDK.Runtime;

namespace CognitiveSDK.Adapters
{
    public class LocalMockEngine
    {
        private readonly CognitiveProfile profile;

        public LocalMockEngine(CognitiveProfile profile)
        {
            this.profile = profile;
        }

        public CognitiveResult Process(string input)
        {
            var normalized = (input ?? string.Empty).ToLowerInvariant();

            if (normalized.Contains("attack") || normalized.Contains("challenge") || normalized.Contains("menace"))
            {
                return new CognitiveResult
                {
                    state = CognitiveState.Aggressive,
                    stability = 0.58f,
                    aggression = 0.82f,
                    intent = "pressure_response",
                };
            }

            if (normalized.Contains("why") || normalized.Contains("how") || normalized.Contains("pourquoi") || normalized.Contains("comment"))
            {
                return new CognitiveResult
                {
                    state = CognitiveState.Analytical,
                    stability = 0.78f,
                    aggression = 0.24f,
                    intent = "analysis",
                };
            }

            if (normalized.Contains("help") || normalized.Contains("ally") || normalized.Contains("aide"))
            {
                return new CognitiveResult
                {
                    state = CognitiveState.Defensive,
                    stability = 0.72f,
                    aggression = 0.18f,
                    intent = "guard",
                };
            }

            return new CognitiveResult
            {
                state = CognitiveState.Neutral,
                stability = profile != null && (profile.mode ?? "remote") == "local" ? 0.74f : 0.5f,
                aggression = 0.12f,
                intent = "observe",
            };
        }
    }
}
