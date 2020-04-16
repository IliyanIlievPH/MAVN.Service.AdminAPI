using System.Collections.Generic;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;

namespace MAVN.Service.AdminAPI.Interfaces.SmartVouchers.Campaigns
{
    public interface ISmartVoucherCampaignWithMobileContentsRequest<T> : ISmartVoucherCampaignBaseRequest where T : IMobileContentRequest
    {
        IEnumerable<T> MobileContents { get; set; }
    }
}
