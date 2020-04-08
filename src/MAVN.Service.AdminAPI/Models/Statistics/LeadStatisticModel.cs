using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Statistics
{
    [PublicAPI]
    public class LeadStatisticModel
    {
        public int NumberOfLeads { get; set; }

        public int NumberOfApprovedLeads { get; set; }

        public int NumberOfUniqueCompletedLeads { get; set; }
    }
}
