using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class CustomersStatisticResponse
    {
        public int TotalActiveCustomers { get; set; }

        public int TotalNonActiveCustomers { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalNewCustomers { get; set; }

        public int TotalRepeatCustomers { get; set; }

        public IReadOnlyList<CustomerStatisticsByDayResponse> NewCustomers { get; set; }
    }
}
