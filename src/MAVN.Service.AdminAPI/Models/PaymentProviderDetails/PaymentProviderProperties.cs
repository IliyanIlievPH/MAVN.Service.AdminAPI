using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Holds payment provider properties
    /// </summary>
    [PublicAPI]
    public class PaymentProviderProperties
    {
        /// <summary>
        /// Properties list
        /// </summary>
        public List<PaymentProviderProperty> Properties { get; set; }

        /// <summary>
        /// Payment provider
        /// </summary>
        public string PaymentProvider { get; set; }
    }
}
