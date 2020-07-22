using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.SetupRequirements
{
    public interface ISetupRequirement
    {
        /// <summary>
        /// Describes what application features this <see cref="ISetupRequirement"/> supports.
        /// </summary>
        public string Capabilities { get; set; }
        /// <summary>
        /// Describes how to fulfill this <see cref="ISetupRequirement"/>.
        /// </summary>
        public string LongDescription { get; set; }
        /// <summary>
        /// Whether this <see cref="ISetupRequirement"/> is required or optional.
        /// </summary>
        public EnforcementType Enforcement { get; set; }
        /// <summary>
        /// Priority in which to check requirements, 1 = top priority, 2 = next, ...
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Current evaluation status of the <see cref="ISetupRequirement"/>.
        /// </summary>
        public RequirementStatus Status { get; set; }
        /// <summary>
        /// Describes in detail the status of the most recent evaluation.
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Evaluates the <see cref="ISetupRequirement"/> and sets Status accordingly.
        /// </summary>
        /// <returns>Returns true if <see cref="ISetupRequirement"/> evaluates to Completed, otherwise returns false.</returns>
        public bool Evaluate();
    }
}
