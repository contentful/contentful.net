using Contentful.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Contentful.Core.Extensions
{
    /// <summary>
    /// Extension methods for Converting Objects to Json
    /// </summary>
    public static class JsonConversionExtensions
    {
        /// <summary>
        /// Converts the object to Json to use when sending a parameter in the body of a request to the Contentful API.
        /// </summary>
        public static string ConvertObjectToJsonString(this object ob)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            resolver.NamingStrategy.OverrideSpecifiedNames = false;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
            };

            settings.Converters.Add(new ExtensionJsonConverter());

            var serializedObject = JsonConvert.SerializeObject(ob, settings);

            return serializedObject;
        }
    }
}