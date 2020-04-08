using System;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class CustomerStatisticsByDayResponse
    {
        public DateTime Day { get; set; }

        public int Count { get; set; }
    }
}
