using System;
using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers
{
    /// <summary>
    /// Request model to present vouchers
    /// </summary>
    public class PresentVouchersRequest
    {
        /// <summary>
        /// Id of the campaign
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Emails of receivers
        /// </summary>
        public List<string> CustomersEmails { get; set; }
    }
}
