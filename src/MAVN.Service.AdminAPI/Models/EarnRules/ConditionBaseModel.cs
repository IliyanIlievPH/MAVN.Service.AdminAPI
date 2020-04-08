using System;
using System.ComponentModel;
using Falcon.Numerics;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    public class ConditionBaseModel : IConversionRateWithRewardTypeHolder
    {
        /// <summary>
        /// The type of the condition.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Represents how many times a condition should be fulfilled before completion.
        /// null for infinity or number greater than 0.
        /// </summary>
        public int? CompletionCount { get; set; }

        /// <summary>
        /// The amount of reward that is going to be granted once the condition is met.
        /// </summary>
        public Money18 ImmediateReward { get; set; }

        [DisplayName(nameof(ImmediateReward))]
        public decimal ImmediateRewardDecimal
        {
            get
            {
                if (decimal.TryParse(ImmediateReward.ToString(), out var num))
                {
                    return num;
                }

                return 0m;
            }
        }

        /// <summary>
        /// The partner's identifier of the condition.
        /// </summary>
        public Guid? PartnerId { get; set; }

        /// <summary>
        /// Identify if the condition has staking
        /// </summary>
        public bool HasStaking { get; set; }

        /// <summary>
        /// Represents stake amount 
        /// </summary>
        public Money18? StakeAmount { get; set; }

        /// <summary>
        /// Represents a staking period
        /// </summary>
        public int? StakingPeriod { get; set; }

        /// <summary>
        /// Represents stake warning period
        /// </summary>
        public int? StakeWarningPeriod { get; set; }

        /// <summary>
        /// Represents staking percentage
        /// </summary>
        public decimal? StakingRule { get; set; }

        /// <summary>
        /// Represents staking burning percent
        /// </summary>
        public decimal? BurningRule { get; set; }

        /// <summary>Indicates the reward type.</summary>
        public RewardType RewardType { get; set; }

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
        /// Indicates if the condition can have reward ratio
        /// </summary>
        public bool RewardHasRatio { get; set; }

        /// <summary>
        /// Represents a display value when percentage reward type is selected
        /// </summary>
        public Money18? ApproximateAward { get; set; }
    }
}
