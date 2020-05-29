using System;
using MAVN.Numerics;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Kyc.Enum;

namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Represents a partner's row response
    /// </summary>
    [PublicAPI]
    public class PartnerRowResponse
    {
        /// <summary>
        /// The partner identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The partner name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The amount in tokens to calculate rate.
        /// </summary>
        public Money18? AmountInTokens { get; set; }

        /// <summary>
        /// The amount in currency to calculate rate.
        /// </summary>
        public decimal? AmountInCurrency { get; set; }

        /// <summary>
        /// Indicates that the global currency rate should be used to convert an amount.
        /// </summary>
        public bool UseGlobalCurrencyRate { get; set; }

        /// <summary>
        /// The partner's creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Represents a name of the user who has created the partner
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Represents the Business Vertical
        /// </summary>
        public BusinessVertical? BusinessVertical { get; set; }

        /// <summary>
        /// Holds possible statuses for KYC
        /// </summary>
        public KycStatus KycStatus { get; set; }
    }
}
