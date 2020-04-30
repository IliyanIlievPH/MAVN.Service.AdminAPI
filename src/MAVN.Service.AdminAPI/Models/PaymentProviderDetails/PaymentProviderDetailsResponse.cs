using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Response model for payment provider details
    /// </summary>
    public class PaymentProviderDetailsResponse
    {
        /// <summary>
        /// Collection of details
        /// </summary>
        public IReadOnlyList<PaymentProviderDetails> PaymentProviderDetails { get; set; }
    }
}
