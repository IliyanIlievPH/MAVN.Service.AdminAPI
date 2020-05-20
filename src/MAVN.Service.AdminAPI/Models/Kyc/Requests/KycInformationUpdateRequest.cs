using System;
using System.ComponentModel.DataAnnotations;
using MAVN.Service.AdminAPI.Models.Kyc.Enum;

namespace MAVN.Service.AdminAPI.Models.Kyc.Requests
{
    /// <summary>Model which holds details about kyc update request</summary>
    public class KycInformationUpdateRequest
    {
        /// <summary>Id of the partner</summary>
        [Required]
        public Guid PartnerId { get; set; }

        /// <summary>Id of the admin user</summary>
        [Required]
        public Guid AdminUserId { get; set; }

        /// <summary>Comment</summary>
        public string Comment { get; set; }

        /// <summary>New status</summary>
        [Required]
        public KycStatus KycStatus { get; set; }
    }
}
