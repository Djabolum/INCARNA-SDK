namespace UnityEngine
{
    public static class JsonUtility
    {
        public static string ToJson(object value) => "{}";
        public static T FromJson<T>(string json) => default;
    }
}
