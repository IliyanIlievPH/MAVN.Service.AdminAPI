using Falcon.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Statistics
{
    /// <summary>
    /// Represents a tokens statistic for period of time.
    /// </summary>
    [PublicAPI]
    public class TokensStatisticsModel
    {
        /// <summary>
        /// The number of earned tokens.
        /// </summary>
        public Money18 EarnedCount { get; set; }

        /// <summary>
        /// The number of burned tokens.
        /// </summary>
        public Money18 BurnedCount { get; set; }

        /// <summary>
        /// The number of all tokens in the wallets of all customers.
        /// </summary>
        public Money18 TotalCount { get; set; }
    }
}
