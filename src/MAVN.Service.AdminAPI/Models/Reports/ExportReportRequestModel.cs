using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Reports
{
    /// <summary>
    /// Represents request for a export report
    /// </summary>
    [PublicAPI]
    public class ExportReportRequestModel
    {
        /// <summary>
        /// Date from
        /// </summary>
        [Required]
        public DateTime From { set; get; }

        /// <summary>
        /// Date to
        /// </summary>
        [Required]
        public DateTime To { set; get; }

        /// <summary>
        /// Partner Id
        /// </summary>
        public Guid? PartnerId { set; get; }

        /// <summary>
        /// TransactionType
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Optional Campaign Id filter
        /// </summary>
        public Guid? CampaignId { get; set; }
    }
}
