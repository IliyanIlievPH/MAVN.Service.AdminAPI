using System;
using System.Collections.Generic;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;
using MAVN.Service.AdminAPI.Interfaces.SmartVouchers.Campaigns;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignBaseRequest<T> : ISmartVoucherCampaignWithMobileContentsRequest<T> where T : IMobileContentRequest
    {
        /// <summary>Voucher campaign name</summary>
        public string Name { get; set; }

        /// <summary>Voucher campaign description</summary>
        public string Description { get; set; }

        /// <summary>Total vouchers count</summary>
        public int VouchersTotalCount { get; set; }

        /// <summary>Voucher price</summary>
        public decimal VoucherPrice { get; set; }

        /// <summary>Voucher price currency</summary>
        public string Currency { get; set; }

        /// <summary>Voucher campaign issuer</summary>
        public string PartnerId { get; set; }

        /// <summary>Voucher campaign start date</summary>
        public DateTime FromDate { get; set; }

        /// <summary>Voucher campaign end date</summary>
        public DateTime? ToDate { get; set; }

        /// <summary>Voucher campaign expiration date</summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>Represents the mobile contents</summary>
        public IEnumerable<T> MobileContents { get; set; }
    }
}
