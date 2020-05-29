using System;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignSetImageRequest
    {
        /// <summary>Image campaign content id</summary>
        public Guid ContentId { get; set; }

        /// <summary>Voucher campaign id</summary>
        public Guid CampaignId { get; set; }

        /// <summary>Voucher campaign id</summary>
        public MobileLocalization Localization { get; set; }
    }
}
