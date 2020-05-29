using System;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers
{
    public class SmartVoucherResponse
    {
        /// <summary>Voucher id</summary>
        public long Id { get; set; }

        /// <summary>Voucher short code</summary>
        public string ShortCode { get; set; }

        /// <summary>Voucher campaign id</summary>
        public Guid CampaignId { get; set; }

        /// <summary>Voucher status</summary>
        public SmartVoucherStatus Status { get; set; }

        /// <summary>Voucher owner id</summary>
        public Guid OwnerId { get; set; }

        /// <summary>Voucher purchase date</summary>
        public DateTime? PurchaseDate { get; set; }

        /// <summary>Voucher redemption date</summary>
        public DateTime? RedemptionDate { get; set; }
    }
}
