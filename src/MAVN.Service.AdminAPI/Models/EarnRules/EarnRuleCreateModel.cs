using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a earn rule creating details.
    /// </summary>
    [PublicAPI]
    public class EarnRuleCreateModel : EarnRuleBaseModel
    {
        /// <summary>
        /// A collection of earn rule conditions.
        /// </summary>
        public IReadOnlyList<ConditionCreateModel> Conditions { get; set; }

        /// <summary>
        /// A collection of earn rule contents.
        /// </summary>
        public IReadOnlyList<MobileContentCreateRequest> MobileContents { get; set; }
    }
}
