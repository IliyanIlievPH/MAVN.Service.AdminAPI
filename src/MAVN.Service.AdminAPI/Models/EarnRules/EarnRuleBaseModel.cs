using System;
using System.ComponentModel;
using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a common properties of earn rule.
    /// </summary>
    [PublicAPI]
    public abstract class EarnRuleBaseModel : IConversionRateWithRewardTypeHolder
    {
        /// <summary>
        /// The name of the earn rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The amount of reward that is going to be granted once all conditions are met.
        /// </summary>
        public Money18 Reward { get; set; }

        [DisplayName(nameof(Reward))]
        public decimal RewardDecimal
        {
            get
            {
                if (decimal.TryParse(Reward.ToString(), out var num))
                {
                    return num;
                }

                return 0m;
            }
        }

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
        /// The reward type of earn rule.
        /// </summary>
        public RewardType RewardType { get; set; }

        /// <summary>
        /// The start date of the earn rule.
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// The end date of the earn rule.
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// The number of earn rule completion.
        /// </summary>
        public int? CompletionCount { get; set; }

        /// <summary>
        /// Indicates that earn rule enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The detailed information of the earn rule.
        /// </summary>
        /// <remarks>
        /// This is plain text, and can include links.
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Represents a display value when percentage reward type is selected
        /// </summary>
        public Money18? ApproximateAward { get; set; }

        /// <summary>
        /// The order of the campaign higher order means lower in the list.
        /// </summary>
        public int Order { get; set; }
    }
}
