using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Request model to check payment integration for specific partner
    /// </summary>
    [PublicAPI]
    public class CheckPaymentIntegrationRequest
    {
        /// <summary>
        /// Id of the partner
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>Payment integration properties</summary>
        public string PaymentIntegrationProperties { get; set; }

        /// <summary>Payment integration provider</summary>
        public string PaymentIntegrationProvider { get; set; }
    }
}
