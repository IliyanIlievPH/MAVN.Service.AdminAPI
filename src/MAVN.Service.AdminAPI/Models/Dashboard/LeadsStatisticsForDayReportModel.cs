using System;
using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class LeadsStatisticsForDayReportModel
    {
        public DateTime Day { get; set; }

        public IReadOnlyCollection<LeadsStatistics> Value { get; set; }

        public int Total { get; set; }
    }
}
