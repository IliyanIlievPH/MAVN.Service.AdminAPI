using System.Collections.Generic;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;

namespace MAVN.Service.AdminAPI.Interfaces.BurnRules
{
    public interface IBurnRuleWithMobileContentsRequest<T> : IBurnRuleBaseRequest where T : IMobileContentRequest
    {
        IEnumerable<T> MobileContents { get; set; }
    }
}
