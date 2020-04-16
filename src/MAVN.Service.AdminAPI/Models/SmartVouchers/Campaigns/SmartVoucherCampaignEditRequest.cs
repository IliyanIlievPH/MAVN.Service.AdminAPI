using System;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignEditRequest : SmartVoucherCampaignBaseRequest<MobileContentEditRequest>
    {
        /// <summary>Voucher campaign id</summary>
        public Guid Id { get; set; }

        /// <summary>Voucher campaign state</summary>
        public SmartVoucherCampaignState State { get; set; }
    }
}
