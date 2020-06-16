using System;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.AuditLogs
{
    public class GetAuditLogsRequest : PagedRequestModel
    {
        /// <summary>From date used for filtering</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>To date used for filtering</summary>
        public DateTime? ToDate { get; set; }

        /// <summary>Admin id used for filtering</summary>
        public Guid? AdminId { get; set; }

        /// <summary>Action type used for filtering</summary>
        public string ActionType { get; set; }
    }
}
