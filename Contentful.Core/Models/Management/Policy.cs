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

    public class AndConstraint : List<IConstraint>, IConstraint
    {
    }

    public class OrConstraint : List<IConstraint>, IConstraint
    {
    }

    public class NotConstraint : IConstraint
    {
        public IConstraint ConstraintToInvert { get; set; }
    }

    public class EqualsConstraint : IConstraint
    {
        public string Property { get; set; }

        public string ValueToEqual { get; set; }
    }

    public class PathConstraint : IConstraint
    {
        public string Fields { get; set; }
    }
}
