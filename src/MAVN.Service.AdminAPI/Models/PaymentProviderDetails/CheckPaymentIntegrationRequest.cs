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
        [Required]
        public Guid PartnerId { get; set; }
    }
}
