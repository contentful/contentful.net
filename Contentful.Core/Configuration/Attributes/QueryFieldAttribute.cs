using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration.Attributes
{
    /// <summary>
    /// An attribute used to designate that a property is a reference field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class QueryFieldAttribute : Attribute
    {
    }
}
