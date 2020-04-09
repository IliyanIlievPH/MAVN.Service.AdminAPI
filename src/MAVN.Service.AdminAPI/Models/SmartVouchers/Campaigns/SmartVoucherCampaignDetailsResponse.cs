using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignDetailsResponse : SmartVoucherCampaignResponse
    {
        /// <summary>Voucher campaigns contents.</summary>
        public List<SmartVoucherCampaignContentResponse> LocalizedContents { get; set; }
    }
}
