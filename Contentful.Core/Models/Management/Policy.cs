using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a policy for actions a <see cref="Role"/> may perform.
    /// </summary>
    public class Policy
    {
        /// <summary>
        /// The effect of this policy.
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// The actions this policy refers to.
        /// </summary>
        [JsonConverter(typeof(StringAllToListJsonConverter))]
        public List<string> Actions { get; set; }
        
        /// <summary>
        /// The constraints imposed.
        /// </summary>
        public IConstraint Constraint { get; set; }
    }

    /// <summary>
    /// Represents a type of constraint that can be imposed by a <see cref="Policy"/>.
    /// </summary>
    [JsonConverter(typeof(ConstraintJsonConverter))]
    public interface IConstraint
    {

    }

    /// <summary>
    /// Represents a list of constraints that evaluates if all its conditions are true.
    /// </summary>
    public class AndConstraint : List<IConstraint>, IConstraint
    {
    }

    /// <summary>
    /// Represents a list of constraints that evaluates if one of its conditions are true.
    /// </summary>
    public class OrConstraint : List<IConstraint>, IConstraint
    {
    }

    /// <summary>
    /// Represents a constraint that inverts the value of the constraint it encapsulates.
    /// </summary>
    public class NotConstraint : IConstraint
    {
        /// <summary>
        /// The constraint to invert.
        /// </summary>
        public IConstraint ConstraintToInvert { get; set; }
    }

    /// <summary>
    /// Represents a constraint that evaluates a property to equal a certain value.
    /// </summary>
    public class EqualsConstraint : IConstraint
    {
        /// <summary>
        /// The property to evaluate.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The value to equal.
        /// </summary>
        public string ValueToEqual { get; set; }
    }

    /// <summary>
    /// Represents a constraint that inverts the value of the constraint it encapsulates.
    /// </summary>
    public class InConstraint : IConstraint
    {
        /// <summary>
        /// The property to evaluate.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The value that must exist in the property.
        /// </summary>
        public string ValueToEqual { get; set; }
    }

    /// <summary>
    /// Represents a constraint that restricts what fields are allowed to be managed.
    /// </summary>
    public class PathConstraint : IConstraint
    {
        /// <summary>
        /// The path expression for which fields to be allowed.
        /// </summary>
        public string Fields { get; set; }
    }
}
