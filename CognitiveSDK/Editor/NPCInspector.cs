#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CognitiveSDK.Editor
{
    public class NPCInspector : EditorWindow
    {
        [MenuItem("Cognitive SDK/NPC Inspector")]
        public static void ShowWindow()
        {
            GetWindow<NPCInspector>("NPC Inspector");
        }

        private void OnGUI()
        {
            GUILayout.Label("Cognitive SDK", EditorStyles.boldLabel);
            GUILayout.Space(8);
            GUILayout.Label("Studio-facing inspector placeholder.");
            GUILayout.Label("Use this panel later for profile validation, runtime preview and debugging.");
        }
    }
}
#endif
