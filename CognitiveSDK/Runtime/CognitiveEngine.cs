using CognitiveSDK.Adapters;

namespace CognitiveSDK.Runtime
{
    public class CognitiveEngine
    {
        private readonly CognitiveProfile profile;
        private readonly HttpBridgeClient bridgeClient;
        private readonly LocalMockEngine localEngine;

        public CognitiveEngine(CognitiveProfile profile)
        {
            this.profile = profile;
            bridgeClient = new HttpBridgeClient(profile);
            localEngine = new LocalMockEngine(profile);
        }

        public CognitiveResult Process(string playerInput)
        {
            if (profile == null)
            {
                return new CognitiveResult
                {
                    state = CognitiveState.Neutral,
                    stability = 0.5f,
                    aggression = 0.0f,
                    intent = "idle",
                };
            }

            if ((profile.mode ?? "remote").ToLowerInvariant() == "local")
            {
                return localEngine.Process(playerInput);
            }

            return bridgeClient.Process(playerInput);
        }

        public CognitiveResult ProcessEmbodied(EmbodiedStepRequest request)
        {
            if (profile == null)
            {
                return Process(string.Empty);
            }

            if ((profile.mode ?? "remote").ToLowerInvariant() == "local")
            {
                return localEngine.Process("embodied_local");
            }

            return bridgeClient.ProcessEmbodied(request);
        }

        public bool ResetSession()
        {
            if (profile == null || (profile.mode ?? "remote").ToLowerInvariant() == "local")
            {
                return true;
            }

            return bridgeClient.ResetSession();
        }

        public InspectResponse InspectEmbodied(string sessionId, string npcId)
        {
            if (profile == null || (profile.mode ?? "remote").ToLowerInvariant() == "local")
            {
                return null;
            }

            return bridgeClient.InspectEmbodied(sessionId, npcId);
        }
    }
}
