using System;
using System.Collections.Generic;
using MAVN.Numerics;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Partners;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents a burn model returned from details request
    /// </summary>
    public class BurnRuleModel
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
        /// Represents the Description of the burn rule
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The business vertical
        /// </summary>
        public BusinessVertical? BusinessVertical { get; set; }

        /// <summary>
        /// The partner identifiers.
        /// </summary>
        public Guid[] PartnerIds { get; set; }

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
        /// The common field to store price. It could be used for voucher price.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// The total number of vouchers.
        /// </summary>
        public int VouchersCount { get; set; }

        /// <summary>
        /// The number of vouchers in stock.
        /// </summary>
        public int VouchersInStockCount { get; set; }

        /// <summary>
        /// Represents the MobileContents of the burn rule
        /// </summary>
        public IReadOnlyList<MobileContentResponse> MobileContents { get; set; }

        /// <summary>
        /// Indicates burn rule's order
        /// </summary>
        public int Order { get; set; }
    }
}
