namespace CognitiveSDK.Runtime
{
    public class CognitiveNPC
    {
        private readonly CognitiveEngine engine;

        public CognitiveNPC(CognitiveProfile profile)
        {
            engine = new CognitiveEngine(profile);
        }

        public CognitiveResult Step(string playerInput)
        {
            return engine.Process(playerInput);
        }

        public CognitiveResult StepEmbodied(EmbodiedStepRequest request)
        {
            return engine.ProcessEmbodied(request);
        }

        public bool ResetSession()
        {
            return engine.ResetSession();
        }

        public InspectResponse InspectEmbodied(string sessionId, string npcId)
        {
            return engine.InspectEmbodied(sessionId, npcId);
        }
    }
}
