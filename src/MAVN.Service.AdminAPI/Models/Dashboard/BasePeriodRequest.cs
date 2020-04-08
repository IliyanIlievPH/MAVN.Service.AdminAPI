using System;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class BasePeriodRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
