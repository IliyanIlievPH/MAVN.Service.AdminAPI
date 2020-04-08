using System;
using System.Collections.Generic;
using Falcon.Numerics;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;
using MAVN.Service.AdminAPI.Interfaces.BurnRules;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Partners;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    public class BurnRuleBaseRequest<T> : IConversionRateHolder, IBurnRuleWithMobileContentsRequest<T> where T : IMobileContentRequest
    {
        /// <summary>
        /// Represents the title of the burn rule that is created
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents the description of the burn rule that is created
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
        /// The common field to store price. It could be used for voucher price.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Indicates that the partner currency rate should be used to convert an amount.
        /// </summary>
        public bool UsePartnerCurrencyRate { get; set; }

        /// <summary>
        /// Represents the mobile contents for the burn rule
        /// </summary>
        public IEnumerable<T> MobileContents { get; set; }

        /// <summary>
        /// Indicates burn rule's order
        /// </summary>
        public int Order { get; set; }
    }
}
