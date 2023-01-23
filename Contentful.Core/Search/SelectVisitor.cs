using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Contentful.Core.Search
{
    internal class SelectVisitor : ExpressionVisitor
    {
        private readonly Type _sourceType;
        private readonly string _prefix;

        public string FieldName { get; private set; }


        public SelectVisitor(Type sourceType) {
            _sourceType = sourceType;
            _prefix = sourceType == typeof(SystemProperties) ? "sys" : "fields";
        }

        public void Reset()
        {
            FieldName = string.Empty;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node?.Expression == null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Expression.NodeType != ExpressionType.Parameter)
            {
                throw new ArgumentException("The only valid member expressions have to be immediate children of the entry type", nameof(node));
            }

            if (node.Type == typeof(SystemProperties))
            {
                throw new ArgumentException("The sys properties cannot be mixed in with the fields properties to select", nameof(node));
            }

            var jsonProp = node.Member.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault();

            var converted = !string.IsNullOrWhiteSpace(jsonProp?.PropertyName) ?
                            jsonProp.PropertyName :
                            node.Member.Name.ToCamelCase();

            FieldName = $"{_prefix}.{converted}";
            return Visit(node.Expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type != _sourceType)
            {
                throw new InvalidOperationException($"The member used must belong to the {_sourceType.Name} type");
            }

            return node;
        }
    }
}
