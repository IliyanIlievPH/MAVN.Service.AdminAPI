using System;
using MAVN.Numerics;
using MAVN.Service.AdminAPI.Models.Partners;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents Burn Rule info model return from list request
    /// </summary>
    public class BurnRuleInfoModel
    {
        /// <summary>
        /// Represents the id of the burn rule
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Represents the title of the burn rule
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The amount in tokens to calculate rate.
        /// </summary>
        public Money18? AmountInTokens { get; set; }

        /// <summary>
        /// The amount in currency to calculate rate.
        /// </summary>
        public decimal? AmountInCurrency { get; set; }

        /// <summary>
        /// Indicates that the partner currency rate should be used to convert an amount.
        /// </summary>
        public bool UsePartnerCurrencyRate { get; set; }

        /// <summary>
        /// Indicates burn rule's creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Indicates burn rule's order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicates burn rule's vertical
        /// </summary>
        public BusinessVertical? Vertical { get; set; }
    }
}
