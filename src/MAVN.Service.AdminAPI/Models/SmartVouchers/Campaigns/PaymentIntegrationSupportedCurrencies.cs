using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class PaymentIntegrationSupportedCurrencies
    {
        /// <summary>Payment provider</summary>
        public string PaymentProvider { get; set; }

        /// <summary>Supported currencies</summary>
        public List<string> SupportedCurrencies { get; set; }
    }
}
