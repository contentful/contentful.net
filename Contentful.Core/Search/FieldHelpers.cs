using Contentful.Core.Configuration.Attributes;
using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Helper methods to get strong typing for querybuilders.
    /// </summary>
    /// <typeparam name="T">The type to get strong typing for.</typeparam>
    public static class FieldHelpers<T>
    {
        /// <summary>
        /// Gets the name of the provided property expression.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="selector">The selector expression.</param>
        /// <returns>The name of the property in lower casing.</returns>
        public static string GetPropertyName<U>(Expression<Func<T, U>> selector)
        {
            var member = selector.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException("Provided expression must be a member type");
            }

            var memberList = new List<string>();

            while (member != null)
            {
                if (member.Type == typeof(SystemProperties))
                {
                    //filtering on sys field.
                    memberList.Add("sys");
                }
                else
                {
                    if(memberList.LastOrDefault() != "sys" && member.Member.CustomAttributes.Any(c => c.AttributeType == typeof(QueryFieldAttribute)))
                    {
                        memberList.Add("fields");
                    }

                    if (member.Member.CustomAttributes.Any(c => c.AttributeType == typeof(JsonPropertyAttribute)))
                    {
                        var attributeData = member.Member.CustomAttributes.First(c => c.AttributeType == typeof(JsonPropertyAttribute));

                        var propertyName = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString();

                        if (propertyName == null)
                        {
                            propertyName = attributeData.NamedArguments.FirstOrDefault(c => c.MemberName == "PropertyName").TypedValue.Value?.ToString();
                        }

                        //Still null, just go with the default.
                        if (propertyName == null)
                        {
                            propertyName = LowerCaseFirstLetterOfString(member.Member.Name);
                        }

                        memberList.Add(propertyName);
                    }
                    else
                    {
                        memberList.Add(LowerCaseFirstLetterOfString(member.Member.Name));
                    }
                }
                member = member.Expression as MemberExpression;
            }

            if (memberList.LastOrDefault() != "fields" && memberList.LastOrDefault() != "sys")
            {
                //We do not have a fields or sys object as root, probably filtering on custom type
                memberList.Add("fields");
            }
            return string.Join(".", memberList.Reverse<string>());
        }

        private static string LowerCaseFirstLetterOfString(string s)
        {
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
