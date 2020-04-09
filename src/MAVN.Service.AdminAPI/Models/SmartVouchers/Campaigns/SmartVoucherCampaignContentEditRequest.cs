using System;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignContentEditRequest : SmartVoucherCampaignContentCreateRequest
    {
        /// <summary>Represents content's identifier</summary>
        public Guid Id { get; set; }
    }
}
