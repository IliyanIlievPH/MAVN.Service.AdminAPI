﻿namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    /// <summary>
    /// Response model
    /// </summary>
    public class PublishedAndActiveCampaignsVouchersCountResponse
    {
        /// <summary>
        /// Total count of vouchers in the system which are for campaigns with status published
        /// </summary>
        public long PublishedCampaignsVouchersTotalCount { get; set; }

        /// <summary>
        /// Total count of vouchers in the system which are for campaigns with status published and are currently active
        /// </summary>
        public long ActiveCampaignsVouchersTotalCount { get; set; }
    }
}
