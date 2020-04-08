using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents the earn rule list request
    /// </summary>
    public class EarnRuleListRequest : PagedRequestModel
    {
        /// <summary>
        /// Earn rule name filter
        /// Optional
        /// </summary>
        public string EarnRuleName { get; set; }
    }
}
