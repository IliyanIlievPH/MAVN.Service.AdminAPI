using Falcon.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Partners
{
    /// <summary>
    /// Represents a base properties of partner. 
    /// </summary>
    [PublicAPI]
    public abstract class PartnerBaseModel
    {
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
        /// The partner's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represents a partner's business vertical
        /// </summary>
        public BusinessVertical BusinessVertical { get; set; } = BusinessVertical.Retail;
    }
}
