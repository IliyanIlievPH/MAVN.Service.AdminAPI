using System;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Request model to edit payment provider details
    /// </summary>
    [PublicAPI]
    public class EditPaymentProviderDetailsRequest : CreatePaymentProviderDetailsRequest
    {
        /// <summary>
        /// Id of the payment provider details
        /// </summary>
        public Guid Id { get; set; }
    }
}
