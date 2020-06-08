using System;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class VoucherDailyStatisticsModel
    {
        /// <summary>date</summary>
        public DateTime Date { get; set; }

        /// <summary>Currency</summary>
        public string Currency { get; set; }

        /// <summary>Sum of all amounts</summary>
        public decimal Sum { get; set; }

        /// <summary>Total count</summary>
        public int Count { get; set; }
    }
}
