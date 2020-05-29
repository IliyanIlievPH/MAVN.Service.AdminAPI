using System.Collections.Generic;
using MAVN.Numerics;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class TokensListResponse
    {
        public IReadOnlyCollection<TokensStatistics> Earn { get; set; }

        public IReadOnlyCollection<TokensStatistics> Burn { get; set; }

        public IReadOnlyCollection<TokensStatistics> WalletBalance { get; set; }

        public Money18 TotalEarn { get; set; }

        public Money18 TotalBurn { get; set; }

        public Money18 TotalWalletBalance { get; set; }
    }
}
