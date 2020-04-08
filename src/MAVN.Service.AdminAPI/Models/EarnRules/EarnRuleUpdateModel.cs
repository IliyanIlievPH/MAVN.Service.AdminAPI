using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a earn rule update details.
    /// </summary>
    [PublicAPI]
    public class EarnRuleUpdateModel : EarnRuleBaseModel
    {
        /// <summary>
        /// The identifier of the earn rule.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A collection of earn rule conditions.
        /// </summary>
        public IReadOnlyList<ConditionUpdateModel> Conditions { get; set; }

        /// <summary>
        /// A collection of earn rule mobile contents.
        /// </summary>
        public IReadOnlyList<MobileContentEditRequest> MobileContents { get; set; }
    }
}
