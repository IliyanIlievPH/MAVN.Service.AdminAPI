using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Reports
{
    /// <summary>
    /// Represents request for a report
    /// </summary>
    [PublicAPI]
    public class ReportRequestModel
    {
        /// <summary>
        /// Date from
        /// </summary>
        public DateTime From { set; get; }
        
        /// <summary>
        /// Date to
        /// </summary>
        public DateTime To { set; get; }

        /// <summary>
        /// Partner Id
        /// </summary>
        public Guid? PartnerId { set; get; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        [Range(1, 1000)]
        public int PageSize { get; set; }

        /// <summary>
        /// The current page index.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; }

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
