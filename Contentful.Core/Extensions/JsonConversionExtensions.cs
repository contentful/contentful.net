using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static readonly CamelCasePropertyNamesContractResolver CamelCasePropertyNamesContractResolver = new()
        {
            NamingStrategy =
            {
                OverrideSpecifiedNames = false
            }
        };
        private static readonly ExtensionJsonConverter ExtensionJsonConverter = new();
        private static readonly JsonSerializer JsonSerializer;
        static JsonConversionExtensions()
        {
            JsonSerializer = new JsonSerializer
            {
                ContractResolver = CamelCasePropertyNamesContractResolver,
            };
            JsonSerializer.Converters.Add(ExtensionJsonConverter);
        }

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
            ContractResolver = CamelCasePropertyNamesContractResolver,
            Converters = [ExtensionJsonConverter],
        };

        /// <summary>
        /// Converts the object to Json to use when sending a parameter in the body of a request to the Contentful API.
        /// </summary>
        public static string ConvertObjectToJsonString(this object ob) => JsonConvert.SerializeObject(ob, SerializerSettings);

        /// <summary>
        /// Converts the object to Json HttpContent to use when sending a parameter in the body of a request to the Contentful API.
        /// </summary>
        /// <remarks>
        /// Avoids converting an object to a JSON UTF-16 string,
        /// that is then converted to a UTF-8 byte array,
        /// that is then converted to a UTF-16 string,
        /// that is then converted to UTF-8 when sending the HttpContent
        /// </remarks>
        internal static NewtonsoftJsonUtf8Content ToNewtonsoftJsonUtf8HttpContent(this object ob, in MediaTypeHeaderValue mediaTypeHeaderValue = null)
            => new(in ob, in JsonSerializer, in mediaTypeHeaderValue);
    }
}