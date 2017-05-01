using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public interface IContentTypeResolver
    {
        Type Resolve(string contentTypeId);
    }
}
