using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Available payment providers requirements response model.
    /// </summary>
    public class AvailablePaymentProvidersRequirementsResponse
    {
        /// <summary>Provider requirements list</summary>
        public List<PaymentProviderProperties> ProvidersRequirements { get; set; }
    }
}
