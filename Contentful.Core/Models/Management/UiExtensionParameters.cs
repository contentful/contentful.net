using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Encapsulates the properties of an UiExtension parameter.
    /// </summary>
    public class UiExtensionParameters
    {
        /// <summary>
        /// The id of the parameter.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of parameter, only allowed values are "Symbol", "Enum", "Number" and "Boolean".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Whether the parameter is required for the extension to work.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Default value to use for the parameter. Must match the type of parameter, i.e. a bool for a "Boolean" type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Default { get; set; }

        /// <summary>
        /// If the parameter is of type "Enum", this property is used to specify the values and labels that should be available as options.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Options { get; set; }

        /// <summary>
        /// Property to customize the labels for the parameter.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Labels { get; set; }
    }

    /// <summary>
    /// Represents the allowed values for types for ui extension parameters.
    /// </summary>
    public class UiExtensionParameterTypes
    {
        /// <summary>
        /// A simple string value.
        /// </summary>
        public const string Symbol = "Symbol";

        /// <summary>
        /// An enumeration of values.
        /// </summary>
        public const string Enum = "Enum";

        /// <summary>
        /// A numeric value.
        /// </summary>
        public const string Number = "Number";

        /// <summary>
        /// A boolean value.
        /// </summary>
        public const string Boolean = "Boolean";
    }
}
