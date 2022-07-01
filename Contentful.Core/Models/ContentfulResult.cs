using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    public readonly record struct ContentfulResult<T>
    {
        public ContentfulResult(string etag, T result)
        {
            Etag = etag;
            Result = result;
        }

        public string Etag { get; }
        public T Result { get; }
    }
}
