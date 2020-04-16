using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignDetailsResponse : SmartVoucherCampaignResponse
    {
        /// <summary>Represents the MobileContents.</summary>
        public IReadOnlyList<MobileContentResponse> MobileContents { get; set; }
    }
}
