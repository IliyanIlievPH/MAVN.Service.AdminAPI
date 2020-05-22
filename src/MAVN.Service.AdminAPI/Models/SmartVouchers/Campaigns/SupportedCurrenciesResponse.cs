using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    /// <summary>
    /// Response model for supported currencies
    /// </summary>
    public class SupportedCurrenciesResponse
    {
        /// <summary>Supported currencies for payment providers</summary>
        public List<PaymentIntegrationSupportedCurrencies> ProvidersSupportedCurrencies { get; set; }
    }
}
