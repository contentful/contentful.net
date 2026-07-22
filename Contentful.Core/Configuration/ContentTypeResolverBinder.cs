using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// An <see cref="ISerializationBinder"/> that only permits <c>$type</c> values to resolve to
    /// CLR types that were explicitly produced by the client's <see cref="IContentTypeResolver"/>.
    /// <para>
    /// <see cref="TypeNameHandling.Auto"/> is required so that <see cref="IContentTypeResolver"/> can
    /// instantiate concrete developer-registered types, but on its own it would let any <c>$type</c>
    /// string in an API response load an arbitrary CLR type (CWE-502 gadget-chain deserialization).
    /// This binder closes that vector: a type is bound only if it was added via <see cref="Allow"/>,
    /// which the client calls exclusively from <c>ResolveContentTypes</c> when it writes a <c>$type</c>
    /// from a developer-configured resolver mapping. Any other <c>$type</c> — including one an attacker
    /// injects into the raw response JSON — is rejected with a <see cref="JsonSerializationException"/>.
    /// </para>
    /// </summary>
    public class ContentTypeResolverBinder : ISerializationBinder
    {
        private readonly ConcurrentDictionary<string, Type> _allowed =
            new ConcurrentDictionary<string, Type>(StringComparer.Ordinal);

        /// <summary>
        /// Registers a type as safe to bind. Call this only for types produced by a developer-configured
        /// <see cref="IContentTypeResolver"/>, never for types derived from raw API response values.
        /// </summary>
        /// <param name="type">The CLR type to allow.</param>
        public void Allow(Type type)
        {
            if (type == null)
            {
                return;
            }

            // Key on both name forms so a $type written as AssemblyQualifiedName resolves the same
            // type regardless of how Newtonsoft splits it into (assemblyName, typeName) on read.
            _allowed[type.AssemblyQualifiedName] = type;
            _allowed[type.FullName] = type;
        }

        /// <summary>
        /// Resolves a <c>$type</c> value to a CLR type, but only if that type was previously allowed via
        /// <see cref="Allow"/>. Unknown types are rejected rather than loaded.
        /// </summary>
        public Type BindToType(string assemblyName, string typeName)
        {
            if (typeName != null)
            {
                if (_allowed.TryGetValue(typeName, out var byFullName))
                {
                    return byFullName;
                }

                var qualified = string.IsNullOrEmpty(assemblyName) ? typeName : $"{typeName}, {assemblyName}";
                if (_allowed.TryGetValue(qualified, out var byQualifiedName))
                {
                    return byQualifiedName;
                }
            }

            throw new JsonSerializationException(
                $"Refusing to deserialize disallowed $type '{typeName}, {assemblyName}'. " +
                "Only types produced by a configured IContentTypeResolver may be resolved.");
        }

        /// <summary>
        /// Emits the type name for a value being serialized. The delivery/preview client never serializes
        /// with <c>$type</c> (the property is injected manually as JSON), so this mirrors the default
        /// Newtonsoft behavior for completeness.
        /// </summary>
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }
    }
}
