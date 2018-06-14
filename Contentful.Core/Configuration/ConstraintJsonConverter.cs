using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.IConstraint"/>.
    /// </summary>
    public class ConstraintJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(IConstraint);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="objectType">The object type to serialize into.</param>
        /// <param name="existingValue">The current value of the property.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The deserialized <see cref="Asset"/>.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);

            return ConvertJsonToConstraint(jObject);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var constraint = value as IConstraint;

            if (constraint == null)
                return;

            serializer.Serialize(writer,  ConvertConstraintToDynamic(constraint));
        }

        private IConstraint ConvertJsonToConstraint(JObject jObject)
        {
            if (jObject["and"] != null)
            {
                var andConstraint = new AndConstraint();
                foreach(var item in jObject["and"])
                {
                    andConstraint.Add(ConvertJsonToConstraint((item as JObject)));
                }

                return andConstraint;
            }
            if(jObject["or"] != null)
            {
                var orConstraint = new OrConstraint();
                foreach (var item in jObject["or"])
                {
                    orConstraint.Add(ConvertJsonToConstraint((item as JObject)));
                }

                return orConstraint;

            }
            if(jObject["equals"] != null)
            {
                var equalsConstraint = new EqualsConstraint();

                equalsConstraint.Property = jObject["equals"][0]["doc"]?.ToString();
                equalsConstraint.ValueToEqual = jObject["equals"][1]?.ToString();

                return equalsConstraint;
            }
            if (jObject["in"] != null)
            {
                var inConstraint = new InConstraint();

                inConstraint.Property = jObject["in"][0]["doc"]?.ToString();
                inConstraint.ValueToEqual = jObject["in"][1]?.ToString();

                return inConstraint;
            }
            if (jObject["paths"] != null)
            {
                var pathsConstraint = new PathConstraint();

                pathsConstraint.Fields = jObject["paths"][0]["doc"]?.ToString();

                return pathsConstraint;
            }
            if (jObject["not"] != null)
            {
                var notConstraint = new NotConstraint();

                notConstraint.ConstraintToInvert = ConvertJsonToConstraint(jObject["not"] as JObject);

                return notConstraint;
            }

            return null;
        }

        private dynamic ConvertConstraintToDynamic(IConstraint constraint)
        {
            if (constraint is AndConstraint)
            {
                return new { and = (constraint as AndConstraint).Select(c => ConvertConstraintToDynamic(c)) };
            }
            if(constraint is OrConstraint)
            {
                return new { or = (constraint as AndConstraint).Select(c => ConvertConstraintToDynamic(c)) };
            }
            if(constraint is EqualsConstraint)
            {
                return new { equals = new dynamic[] { new { doc = (constraint as EqualsConstraint).Property }, (constraint as EqualsConstraint).ValueToEqual } };
            }
            if(constraint is NotConstraint)
            {
                return new { not = ConvertConstraintToDynamic((constraint as NotConstraint).ConstraintToInvert) };
            }
            if (constraint is PathConstraint)
            {
                return new { paths = new dynamic[] { new { doc = (constraint as PathConstraint).Fields } } };
            }
            if (constraint is InConstraint)
            {
                return new { @in = new dynamic[] { new { doc = (constraint as InConstraint).Property }, (constraint as InConstraint).ValueToEqual } };
            }

            return null;
        }
    }
}
