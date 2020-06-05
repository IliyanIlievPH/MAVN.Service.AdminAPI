using System;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class CustomersListRequest: BasePeriodRequest
    {
        /// <summary>Ids of partners, used for filtering</summary>
        public Guid[] PartnerIds { get; set; }
    }
}
