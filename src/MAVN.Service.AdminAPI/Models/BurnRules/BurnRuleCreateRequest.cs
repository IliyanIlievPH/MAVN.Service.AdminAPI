using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents burn rule create request
    /// </summary>
    [PublicAPI]
    public class BurnRuleCreateRequest : BurnRuleBaseRequest<MobileContentCreateRequest>
    {
    }
}
