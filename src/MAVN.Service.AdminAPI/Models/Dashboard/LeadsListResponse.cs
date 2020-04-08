using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class LeadsListResponse
    {
        public IReadOnlyCollection<LeadsStatisticsForDayReportModel> Leads { get; set; }

        public int TotalNumber { get; set; }
    }
}
