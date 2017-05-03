using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// An interface for a contenttype resolver to allow resolving contenttypes to concrete CLR types.
    /// </summary>
    public interface IContentTypeResolver
    {
        /// <summary>
        /// Resolves a content type id to a concrete type.
        /// </summary>
        /// <param name="contentTypeId">The contenttype id to resolve.</param>
        /// <returns>The type for the provided contenttype id.</returns>
        Type Resolve(string contentTypeId);
    }
}
