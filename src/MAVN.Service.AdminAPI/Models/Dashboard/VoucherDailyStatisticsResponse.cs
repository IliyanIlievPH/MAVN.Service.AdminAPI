using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class VoucherDailyStatisticsResponse
    {
        /// <summary>Statistics for bought vouchers per day and currency</summary>
        public IReadOnlyList<VoucherDailyStatisticsModel> BoughtVoucherStatistics { get; set; }

        /// <summary>Statistics for used vouchers per day and currency</summary>
        public IReadOnlyList<VoucherDailyStatisticsModel> UsedVoucherStatistics { get; set; }
    }
}
