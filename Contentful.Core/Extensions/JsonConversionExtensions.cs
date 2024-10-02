using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Contentful.Core.Extensions
{
    /// <summary>
    /// Extension methods for Converting Objects to Json
    /// </summary>
    public static class JsonConversionExtensions
    {
        /// <summary>Deserializes an object of type T from the response</summary>
        /// <param name="response">The response to deserialize from</param>
        /// <param name="serializer">The serializer to use</param>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <returns>The deserialized object</returns>
        /// <exception cref="ArgumentNullException">Thrown if the response is null</exception>
        /// <exception cref="ArgumentException">Thrown if response.Content is null</exception>
        public static async Task<T> GetObjectFromResponse<T>(this HttpResponseMessage response, JsonSerializer serializer)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (response.Content == null)
                throw new ArgumentException(nameof(response.Content) + " is null", nameof(response));
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);
            return serializer.Deserialize<T>(jsonReader);
        }

        /// <summary>Deserializes a JObject from the response</summary>
        /// <param name="response">The response to deserialize from</param>
        /// <returns>The deserialized JObject</returns>
        /// <exception cref="ArgumentNullException">Thrown if the response is null</exception>
        /// <exception cref="ArgumentException">Thrown if response.Content is null</exception>
        public static async Task<JObject> GetJObjectFromResponse(this HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (response.Content == null)
                throw new ArgumentException(nameof(response.Content) + " is null", nameof(response));
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);
            return await JObject.LoadAsync(jsonReader).ConfigureAwait(false);
        }

        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy =
                {
                    OverrideSpecifiedNames = false
                }
            },
            Converters = new List<JsonConverter>
            {
                new ExtensionJsonConverter(),
            },
        };

        /// <summary>
        /// Converts the object to Json to use when sending a parameter in the body of a request to the Contentful API.
        /// </summary>
        public static string ConvertObjectToJsonString(this object ob) => JsonConvert.SerializeObject(ob, SerializerSettings);
    }
}