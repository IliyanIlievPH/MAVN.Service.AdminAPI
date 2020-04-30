using System;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Request model to create payment provider details
    /// </summary>
    public class CreatePaymentProviderDetailsRequest
    {
        /// <summary>The id of the partner</summary>
        [Required]
        public Guid PartnerId { get; set; }

        /// <summary>the payment provider used</summary>
        [Required]
        public string PaymentIntegrationProvider { get; set; }

        /// <summary>
        /// Configuration properties for the payment provider (json)
        /// </summary>
        [Required]
        public string PaymentIntegrationProperties { get; set; }
    }
}
