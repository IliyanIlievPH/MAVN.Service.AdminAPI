using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents burn rule list request
    /// </summary>
    public class BurnRuleListRequest : PagedRequestModel
    {
        /// <summary>
        /// Title filter
        /// Optional
        /// </summary>
        public string Title { get; set; }
    }
}
