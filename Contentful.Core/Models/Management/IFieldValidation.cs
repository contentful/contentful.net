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

        public LinkContentTypeValidator(params string[] contentTypeIds) : this(contentTypeIds.AsEnumerable())
        {

        }

        public LinkContentTypeValidator(IEnumerable<ContentType> contentTypes) : this(contentTypes.Select(c => c.SystemProperties.Id))
        {

        }

        public LinkContentTypeValidator(IEnumerable<string> contentTypeIds)
        {
            EntryIds = new List<string>(contentTypeIds);
        }

        public object CreateValidator()
        {
            return new { linkContentType = EntryIds };
        }
    }

    public class InValuesValidator : IFieldValidator
    {
        public List<string> RequiredValues { get; set; }

        public InValuesValidator(params string[] requiredValues) : this(requiredValues.AsEnumerable())
        {

        }

        public InValuesValidator(IEnumerable<string> requiredValues)
        {
            RequiredValues = new List<string>(requiredValues);
        }

        public object CreateValidator()
        {
            return new { @in = RequiredValues };
        }
    }

    public class MimeTypeValidator : IFieldValidator
    {
        public MimeTypeRestriction MimeType { get; set; }

        public MimeTypeValidator(MimeTypeRestriction mimeType)
        {
            MimeType = mimeType;
        }

        public object CreateValidator()
        {
            return new { linkMimetypeGroup = MimeType.ToString() };
        }
    }

    public class SizeValidator : IFieldValidator
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public SizeValidator(int? min, int? max)
        {
            Min = min;
            Max = max;
        }

        public object CreateValidator()
        {
            return new { size = new { min = Min, max = Max } };
        }
    }

    public class RangeValidator : IFieldValidator
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public RangeValidator(int? min, int? max)
        {
            Min = min;
            Max = max;
        }

        public object CreateValidator()
        {
            return new { size = new { min = Min, max = Max } };
        }
    }

    public class RegexValidator : IFieldValidator
    {
        public string Expression { get; set; }
        public string Flags { get; set; }

        public RegexValidator(string expression, string flags)
        {
            Expression = expression;
            Flags = flags;
        }

        public object CreateValidator()
        {
            return new { regexp = new { pattern = Expression, flags = Flags } };
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
