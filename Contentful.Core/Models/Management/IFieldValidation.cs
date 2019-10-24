using Contentful.Core.Configuration;
using Contentful.Core.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
    /// Enumeration for the possible rich text marks.
    /// </summary>
    public enum EnabledMarkRestrictions
    {
        /// <summary>
        /// Bold.
        /// </summary>
        bold,
        /// <summary>
        /// Italic.
        /// </summary>
        italic,
        /// <summary>
        /// Underline.
        /// </summary>
        underline,
        /// <summary>
        /// Code.
        /// </summary>
        code
    }

    /// <summary>
    /// Represents a validator that validates the EnabledMarks property.
    /// </summary>
    /// <summary>
    /// Represents a validator that validates the Enabled marks />
    /// </summary>
    public class EnabledNodeTypesValidator : IFieldValidator
    {

        /// <summary>
        /// The marks to validate against.
        /// </summary>
        public List<string> EnabledNodeTypes { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EnabledNodeTypesValidator"/>.
        /// </summary>
        /// <param name="enabledNodeTypes">The node types to validate against in string format.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public EnabledNodeTypesValidator(IEnumerable<string> enabledNodeTypes, string message = null)
        {
            EnabledNodeTypes = enabledNodeTypes?
                .ToList();
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { enabledNodeTypes = EnabledNodeTypes?.Select(c => c?.ToLower()), message = Message };
        }
    }



    /// <summary>
    /// Represents a validator that validates the EnabledMarks property.
    /// </summary>
    /// <summary>
    /// Represents a validator that validates the Enabled marks />
    /// </summary>
    public class EnabledMarksValidator : IFieldValidator
    {
        /// <summary>
        /// The marks to validate against.
        /// </summary>
        public List<EnabledMarkRestrictions> EnabledMarks { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EnabledMarksValidator"/>.
        /// </summary>
        /// <param name="enabledMarks">The marks to validate against.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public EnabledMarksValidator(IEnumerable<EnabledMarkRestrictions> enabledMarks, string message = null)
        {
            EnabledMarks = enabledMarks?.ToList();
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { enabledMarks = EnabledMarks.Select(c => c.ToString()?.ToLower()), message = Message };
        }
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
        public InValuesValidator(string message = null, params string[] requiredValues) : this(requiredValues.AsEnumerable(), message)
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
            return new { linkMimetypeGroup = MimeTypes.Select(c => c.ToString()?.ToLower()), message = Message };
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
    /// Represents a validator that validates a field value to prohibit a certain regular expression.
    /// </summary>
    public class ProhibitRegexValidator : IFieldValidator
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
        /// Initializes a new instance of <see cref="ProhibitRegexValidator"/>.
        /// </summary>
        /// <param name="expression">The regular expression to validate against.</param>
        /// <param name="flags">The flags to apply to the regular expression.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public ProhibitRegexValidator(string expression, string flags, string message = null)
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
            return new { prohibitRegexp = new { pattern = Expression, flags = Flags }, message = Message };
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

    /// <summary>
    /// Represents a validator that ensures that the field value is within a certain date range.
    /// </summary>  
    public class DateRangeValidator : IFieldValidator
    {
        private string _min;

        /// <summary>
        /// The minimum allowed date.
        /// </summary>
        public DateTime? Min
        {
            get
            {
                if (DateTime.TryParse(_min, out DateTime parsed))
                    return (DateTime?)parsed;

                return null;
            }
        }

        private string _max;

        /// <summary>
        /// The maximum allowed date.
        /// </summary>
        public DateTime? Max
        {
            get
            {
                if (DateTime.TryParse(_max, out DateTime parsed))
                    return (DateTime?)parsed;

                return null;
            }
        }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Initializes a new instance of <see cref="DateRangeValidator"/>.
        /// </summary>
        /// <param name="min">The minimum date of the range.</param>
        /// <param name="max">The maximum date of the range.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public DateRangeValidator(string min, string max, string message = null)
        {
            _min = min;
            _max = max;
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
		public object CreateValidator()
        {
            return new { dateRange = new { min = _min, max = _max }, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that ensures that the media file size is within a certain size range.
    /// </summary>      
    public class FileSizeValidator : IFieldValidator
    {
        private const int BYTES_IN_KB = 1024;
        private const int BYTES_IN_MB = 1048576;
        /// <summary>
        /// The minimum allowed size of the file.
        /// </summary>
        public int? Min { get; set; }

        /// <summary>
        /// The maximum allowed size of the file.
        /// </summary>
        public int? Max { get; set; }
        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSizeValidator" />.
        /// </summary>
        /// <param name="min">The minimum size of the file.</param>
        /// <param name="max">The maximum size of the file.</param>
        /// <param name="minUnit">The unit measuring the minimum file size.</param>
        /// <param name="maxUnit">The unit measuring the maximum file size.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public FileSizeValidator(int? min, int? max, string minUnit = SystemFileSizeUnits.Bytes, string maxUnit = SystemFileSizeUnits.Bytes, string message = null)
        {
            Min = GetCalculatedByteSize(min, minUnit);
            Max = GetCalculatedByteSize(max, maxUnit);
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
		public object CreateValidator()
        {
            return new { assetFileSize = new { min = Min, max = Max }, message = Message };
        }


        private int? GetCalculatedByteSize(int? value, string unit)
        {
            if (value != null)
            {
                if (unit == SystemFileSizeUnits.KB)
                    value = value * BYTES_IN_KB;
                if (unit == SystemFileSizeUnits.MB)
                    value = value * BYTES_IN_MB;
            }
            return value;
        }
    }

    /// <summary>
    /// Represents a validator that ensures that the image dimensions are within a certain range.
    /// </summary>   
    public class ImageSizeValidator : IFieldValidator
    {
        /// <summary>
        /// The minimum allowed width of the image (in px).
        /// </summary>
        public int? MinWidth { get; set; }

        /// <summary>
        /// The maximum allowed width of the image (in px).
        /// </summary>
        public int? MaxWidth { get; set; }
        /// <summary>
        /// The minimum allowed height of the iamge (in px).
        /// </summary>
        int? MinHeight { get; set; }

        /// <summary>
        /// The maximum allowed height of the image (in px).
        /// </summary>
        public int? MaxHeight { get; set; }

        /// <summary>
        /// The custom error message that should be displayed.
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Initializes a new instance of <see cref="ImageSizeValidator" />.
        /// </summary>
        /// <param name="minWidth">The minimum width of the image.</param>
        /// <param name="maxWidth">The maximum width of the image.</param>
        /// <param name="minHeight">The minimum height of the image.</param>
        /// <param name="maxHeight">The maximum height of the image.</param>
        /// <param name="message">The custom error message for this validation.</param>
        public ImageSizeValidator(int? minWidth, int? maxWidth, int? minHeight, int? maxHeight, string message = null)
        {
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            Message = message;
        }

        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
		public object CreateValidator()
        {
            return new { assetImageDimensions = new { width = new { min = MinWidth, max = MaxWidth }, height = new { min = MinHeight, max = MaxHeight } }, message = Message };
        }
    }

    /// <summary>
    /// Represents a validator that validates the nodes of a rich text field.
    /// </summary>
    public class NodesValidator : IFieldValidator
    {
        /// <summary>
        /// A list of validations applied to entry hyper links.
        /// </summary>
        [JsonProperty("entry-hyperlink")]
        public IEnumerable<IFieldValidator> EntryHyperlink { get; set; }
        /// <summary>
        /// A list of validations applied to embedded entry blocks.
        /// </summary>
        [JsonProperty("embedded-entry-block")]
        public IEnumerable<IFieldValidator> EmbeddedEntryBlock { get; set; }
        /// <summary>
        /// A list of validations applied to inline embedded entries.
        /// </summary>
        [JsonProperty("embedded-entry-inline")]
        public IEnumerable<IFieldValidator> EmbeddedEntryInline { get; set; }


        /// <summary>
        /// Creates a representation of this validator that can be easily serialized.
        /// </summary>
        /// <returns>The object to serialize.</returns>
        public object CreateValidator()
        {
            return new { nodes = new Nodes(this) };
        }
    }

    internal class Nodes
    {
        public Nodes(NodesValidator validator)
        {
            EntryHyperlink = validator.EntryHyperlink;
            EmbeddedEntryBlock = validator.EmbeddedEntryBlock;
            EmbeddedEntryInline = validator.EmbeddedEntryInline;
        }

        [JsonProperty("entry-hyperlink")]
        public IEnumerable<IFieldValidator> EntryHyperlink { get; set; }

        [JsonProperty("embedded-entry-block")]
        public IEnumerable<IFieldValidator> EmbeddedEntryBlock { get; set; }

        [JsonProperty("embedded-entry-inline")]
        public IEnumerable<IFieldValidator> EmbeddedEntryInline { get; set; }
    }
}
