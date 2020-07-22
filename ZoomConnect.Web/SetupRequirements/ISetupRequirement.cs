using System;

namespace ZoomConnect.Web.SetupRequirements
{
    public interface ISetupRequirement
    {
        /// <summary>
        /// Shows what resource this requirement is for.
        /// </summary>
        public RequirementType Type { get; }
        /// <summary>
        /// Describes what application features this <see cref="ISetupRequirement"/> supports.
        /// </summary>
        public string Capabilities { get; }
        /// <summary>
        /// Describes how to fulfill this <see cref="ISetupRequirement"/>.
        /// </summary>
        public string LongDescription { get; }
        /// <summary>
        /// Whether this <see cref="ISetupRequirement"/> is required or optional.
        /// </summary>
        public EnforcementType Enforcement { get; }
        /// <summary>
        /// Priority in which to check requirements, 1 = top priority, 2 = next.
        /// Failures in higher priority requirements short-circuit the automatic evaluation
        /// of lower priority requirements.
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// Current evaluation status of the <see cref="ISetupRequirement"/>.
        /// </summary>
        public RequirementStatus Status { get; }
        /// <summary>
        /// Describes in detail the status of the most recent evaluation.
        /// </summary>
        public string StatusDescription { get; }
        /// <summary>
        /// Evaluates the <see cref="ISetupRequirement"/> and sets Status accordingly.
        /// </summary>
        /// <returns>Returns true if <see cref="ISetupRequirement"/> evaluates to Completed, otherwise returns false.</returns>
        public bool Evaluate();
    }
}
