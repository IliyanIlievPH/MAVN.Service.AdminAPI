using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Statistics
{
    /// <summary>
    /// Represents a customer statistic for period of time.
    /// </summary>
    [PublicAPI]
    public class CustomersStatisticsModel
    {
        /// <summary>
        /// The number of active customers.
        /// </summary>
        public int ActiveCount { get; set; }

        /// <summary>
        /// The number of customers that registered in a specific period of time.
        /// </summary>
        public int RegistrationsCount { get; set; }

        /// <summary>
        /// The number of all customers.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
