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
    [JsonConverter(typeof(ValidationsJsonConverter))]
    public interface IFieldValidator
    {
        object CreateValidator();
    }

    public class LinkContentTypeValidator : IFieldValidator
    {
        public List<string> EntryIds { get; set; }
        public string Message { get; set; }

        public LinkContentTypeValidator(string message = null, params string[] contentTypeIds) : this(contentTypeIds.AsEnumerable())
        {

        }

        public LinkContentTypeValidator(IEnumerable<ContentType> contentTypes, string message = null) : this(contentTypes.Select(c => c.SystemProperties.Id))
        {

        }

        public LinkContentTypeValidator(IEnumerable<string> contentTypeIds, string message = null)
        {
            EntryIds = new List<string>(contentTypeIds);
            Message = message;
        }

        public object CreateValidator()
        {
            return new { linkContentType = EntryIds, message = Message };
        }
    }

    public class InValuesValidator : IFieldValidator
    {
        public List<string> RequiredValues { get; set; }
        public string Message { get; set; }

        public InValuesValidator(string message = null, params string[] requiredValues) : this(requiredValues.AsEnumerable())
        {

        }

        public InValuesValidator(IEnumerable<string> requiredValues, string message = null)
        {
            RequiredValues = new List<string>(requiredValues);
            Message = message;
        }

        public object CreateValidator()
        {
            return new { @in = RequiredValues, message = Message };
        }
    }

    public class MimeTypeValidator : IFieldValidator
    {
        public MimeTypeRestriction MimeType { get; set; }
        public string Message { get; set; }

        public MimeTypeValidator(MimeTypeRestriction mimeType, string message = null)
        {
            MimeType = mimeType;
            Message = message;
        }

        public object CreateValidator()
        {
            return new { linkMimetypeGroup = MimeType.ToString(), message = Message };
        }
    }

    public class SizeValidator : IFieldValidator
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string Message { get; set; }

        public SizeValidator(int? min, int? max, string message = null)
        {
            Min = min;
            Max = max;
            Message = message;
        }

        public object CreateValidator()
        {
            return new { size = new { min = Min, max = Max }, message = Message };
        }
    }

    public class RangeValidator : IFieldValidator
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string Message { get; set; }

        public RangeValidator(int? min, int? max, string message = null)
        {
            Min = min;
            Max = max;
            Message = message;
        }

        public object CreateValidator()
        {
            return new { size = new { min = Min, max = Max }, message = Message };
        }
    }

    public class RegexValidator : IFieldValidator
    {
        public string Expression { get; set; }
        public string Flags { get; set; }
        public string Message { get; set; }

        public RegexValidator(string expression, string flags, string message = null)
        {
            Expression = expression;
            Flags = flags;
            Message = message;
        }

        public object CreateValidator()
        {
            return new { regexp = new { pattern = Expression, flags = Flags }, message = Message };
        }
    }

    public class UniqueValidator : IFieldValidator
    {
        public object CreateValidator()
        {
            return new { unique = true };
        }
    }

}
