using Newtonsoft.Json;

namespace BusinessIntegrationClient
{
    /// <summary>
    ///     Helper methods to convert objects to/from JSON.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        ///     Converts an object to a JSON string representation.
        /// </summary>        
        /// <param name="obj"></param>
        /// <returns>JSON</returns>
        public static string ToJson(this object obj)
        {
            if (obj == null || string.Empty.Equals(obj)) return obj as string;
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,                
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        ///     Serializes JSON in a string into an instance of type <see cref="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}