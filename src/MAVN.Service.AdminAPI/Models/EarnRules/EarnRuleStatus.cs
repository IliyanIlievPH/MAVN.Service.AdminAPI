using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Specifies a earn rule statuses.
    /// </summary>
    [PublicAPI]
    public enum EarnRuleStatus
    {
        /// <summary>
        /// Unspecified status.
        /// </summary>
        None,

        /// <summary>
        /// Represents status of the earn rule that has not begin started yet.
        /// </summary>
        Pending,

        /// <summary>
        /// Represents status of the earn rule that is currently active.
        /// </summary>
        Active,

        /// <summary>
        /// Represents status of the earn rule that is already completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Represents status of the earn rule that has been disabled.
        /// </summary>
        Inactive
    }
}
