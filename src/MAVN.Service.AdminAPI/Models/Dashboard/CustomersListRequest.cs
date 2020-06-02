using System;

namespace MAVN.Service.AdminAPI.Models.Dashboard
{
    public class CustomersListRequest: BasePeriodRequest
    {
        /// <summary>
        /// PartnerId used for filtering
        /// </summary>
        public Guid? PartnerId { get; set; }
    }
}
