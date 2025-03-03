using UnityEngine;

namespace Framework.SaveSystem.Helpers
{
    public static class JsonHelper
    {
        public static T[] FromJsonArray<T>(string json)
        {
            string newJson = $"{{\"array\":{json}}}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        public static string ToJsonArray<T>(T[] array, bool prettyPrint = false)
        {
            Wrapper<T> wrapper = new Wrapper<T>
            {
                array = array
            };
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}