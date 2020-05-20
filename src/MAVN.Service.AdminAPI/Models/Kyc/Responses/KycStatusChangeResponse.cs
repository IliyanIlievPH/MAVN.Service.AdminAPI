using System;
using MAVN.Service.AdminAPI.Models.Kyc.Enum;

namespace MAVN.Service.AdminAPI.Models.Kyc.Responses
{
    /// <summary>holds info about kyc status changes</summary>
    public class KycStatusChangeResponse
    {
        /// <summary>Id of the partner</summary>
        public Guid PartnerId { get; set; }

        /// <summary>Id of the admin user</summary>
        public Guid AdminUserId { get; set; }

        /// <summary>Timestamp</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>comment</summary>
        public string Comment { get; set; }

        /// <summary>previous kyc status</summary>
        public KycStatus? OldKycStatus { get; set; }

        /// <summary>new kyc status</summary>
        public KycStatus NewKycStatus { get; set; }
    }
}
