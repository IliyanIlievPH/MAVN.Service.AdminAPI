using System;

namespace MAVN.Service.AdminAPI.Models.AuditLogs
{
    /// <summary>Response model for audit log</summary>
    public class AuditLogResponse
    {
        /// <summary>Id of the admin user</summary>
        public Guid AdminUserId { get; set; }

        /// <summary>Action context in json format</summary>
        public string ActionContextJson { get; set; }

        /// <summary>Timestamp</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Action type</summary>
        public string ActionType { get; set; }
    }
}
