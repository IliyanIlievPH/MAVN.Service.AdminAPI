using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents burn rule update model
    /// </summary>
    public class BurnRuleUpdateRequest : BurnRuleBaseRequest<MobileContentEditRequest>
    {
        /// <summary>
        /// Represents the id of the burn rule
        /// </summary>
        public string Id { get; set; }
    }
}
