using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers
{
    /// <summary>
    /// response model for vouchers present
    /// </summary>
    public class PresentSmartVouchersResponse
    {
        /// <summary>
        /// List of emails which are not registered as customers in the system
        /// </summary>
        public List<string> NotRegisteredEmails { get; set; }
    }
}
