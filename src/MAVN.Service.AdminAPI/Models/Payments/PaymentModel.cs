using System;
using Falcon.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Payments
{
    /// <summary>
    /// Represents a payment.
    /// </summary>
    [PublicAPI]
    public class PaymentModel
    {
        /// <summary>
        /// The payment unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The campaign name.
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// The invoice identifier.
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// The customer email.
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// The amount in tokens.
        /// </summary>
        public Money18 AmountInTokens { get; set; }

        /// <summary>
        /// The amount in the target currency.
        /// </summary>
        public Money18 AmountInCurrency { get; set; }

        /// <summary>
        /// The payment timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
