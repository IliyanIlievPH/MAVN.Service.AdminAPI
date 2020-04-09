using System;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignSetImageRequest
    {
        /// <summary>Image campaign content id</summary>
        public string Id { get; set; }

        /// <summary>Voucher campaign id</summary>
        public Guid CampaignId { get; set; }

        /// <summary>Voucher campaign id</summary>
        public Localization Localization { get; set; }

        /// <summary>Image file name</summary>
        public string Name { get; set; }

        /// <summary>Image file type</summary>
        public string Type { get; set; }

        /// <summary>Image file content</summary>
        public byte[] Content { get; set; }
    }
}
