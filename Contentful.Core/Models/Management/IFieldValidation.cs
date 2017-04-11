using Contentful.Core.Configuration;
using Contentful.Core.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a validator of a field for a <see cref="ContentType"/>.
    /// </summary>
    [JsonConverter(typeof(ValidationsJsonConverter))]
    public interface IFieldValidator
    {
        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        object CreateValidator();
    }

    /// <summary>
    /// Represents a validator that validates what type of <see cref="ContentType"/> you can link to from a field.
    /// </summary>
    public class LinkContentTypeValidator : IFieldValidator
    {
        /// <summary>
        /// The id of the content types allowed.
        /// </summary>
        public List<string> ContentTypeIds { get; set; }
        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="LinkContentTypeValidator"/>.
        /// </summary>
        /// <param name="message">The custom error message for this validation.</param>
        /// <param name="contentTypeIds">The content types to restrict to.</param>
        public LinkContentTypeValidator(string message = null, params string[] contentTypeIds) : this(contentTypeIds.AsEnumerable(), message)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="LinkContentTypeValidator"/>.
        /// </summary>
        /// <param name="contentTypes">The content types to restrict to.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public LinkContentTypeValidator(IEnumerable<ContentType> contentTypes, string message = null) : this(contentTypes.Select(c => c.SystemProperties.Id), message)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="LinkContentTypeValidator"/>.
        /// </summary>
        /// <param name="contentTypeIds">The content type ids to restrict to.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public LinkContentTypeValidator(IEnumerable<string> contentTypeIds, string message = null)
        {
            ContentTypeIds = new List<string>(contentTypeIds);
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { linkContentType = ContentTypeIds, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates that a field value is in a predefined set of values.
    /// </summary>
    public class InValuesValidator : IFieldValidator
    {
        /// <summary>
        /// The list of values to compare the field value to.
        /// </summary>
        public List<string> RequiredValues { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="InValuesValidator"/>
        /// </summary>
        /// <param name="message">The custom error message for this validation.</param>
        /// <param name="requiredValues">The values to validate the field value against.</param>
        public InValuesValidator(string message = null, params string[] requiredValues) : this(requiredValues.AsEnumerable())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="InValuesValidator"/>
        /// </summary>
        /// <param name="requiredValues">The values to validate the field value against.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public InValuesValidator(IEnumerable<string> requiredValues, string message = null)
        {
            RequiredValues = new List<string>(requiredValues);
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { @in = RequiredValues, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates that an asset is of a certain <see cref="MimeTypeRestriction"/>
    /// </summary>
    public class MimeTypeValidator : IFieldValidator
    {
        /// <summary>
        /// The mime type groups to validate against.
        /// </summary>
        public List<MimeTypeRestriction> MimeTypes { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="MimeTypeValidator"/>.
        /// </summary>
        /// <param name="mimeTypes">The mime type groups to validate against.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public MimeTypeValidator(IEnumerable<MimeTypeRestriction> mimeTypes, string message = null)
        {
            MimeTypes = mimeTypes?.ToList();
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { linkMimetypeGroup = MimeTypes.Select(c => c.ToString()), message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates that an array of items conforms to a certain size.
    /// </summary>
    public class SizeValidator : IFieldValidator
    {
        /// <summary>
        /// The minimum number of items in the array.
        /// </summary>
        public int? Min { get; set; }

        /// <summary>
        /// The maximum number of items in the array.
        /// </summary>
        public int? Max { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SizeValidator"/>.
        /// </summary>
        /// <param name="min">The minimum number of items in the array.</param>
        /// <param name="max">The maximum number of items in the array.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public SizeValidator(int? min, int? max, string message = null)
        {
            Min = min;
            Max = max;
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { size = new { min = Min, max = Max }, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates that a field value is within a certain range.
    /// </summary>
    public class RangeValidator : IFieldValidator
    {
        /// <summary>
        /// The minimum number for the range.
        /// </summary>
        public int? Min { get; set; }
        /// <summary>
        /// The maximum number for the range.
        /// </summary>
        public int? Max { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="RangeValidator"/>.
        /// </summary>
        /// <param name="min">The minimum number for the range.</param>
        /// <param name="max">The maximum naumber for the range.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public RangeValidator(int? min, int? max, string message = null)
        {
            Min = min;
            Max = max;
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { range = new { min = Min, max = Max }, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates a field value to conform to a certain regular expression.
    /// </summary>
    public class RegexValidator : IFieldValidator
    {
        /// <summary>
        /// The expression to use.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The flags to apply to the regular expression.
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="RegexValidator"/>.
        /// </summary>
        /// <param name="expression">The regular expression to validate against.</param>
        /// <param name="flags">The flags to apply to the regular expression.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public RegexValidator(string expression, string flags, string message = null)
        {
            Expression = expression;
            Flags = flags;
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { regexp = new { pattern = Expression, flags = Flags }, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that ensures that the field value is unique among all entries at the time of publishing.
    /// </summary>
    public class UniqueValidator : IFieldValidator
    {
        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { unique = true };
        }
    }

}
