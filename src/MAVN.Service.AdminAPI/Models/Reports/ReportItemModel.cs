using System;
using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Reports
{
    /// <summary>
    /// Represents transactions report item
    /// </summary>
    [PublicAPI]
    public class ReportItemModel
    {
        /// <summary>Id</summary>
        public Guid Id { get; set; }

        /// <summary>Timestamp</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Amount</summary>
        public Money18 Amount { get; set; }

        /// <summary>Transaction type</summary>
        public string TransactionType { get; set; }

        /// <summary>Transaction status</summary>
        public string Status { get; set; }

        /// <summary>Vertical</summary>
        public string Vertical { get; set; }

        /// <summary>Transaction category</summary>
        public string TransactionCategory { get; set; }

        /// <summary>Campaign name</summary>
        public string CampaignName { get; set; }

        /// <summary>Campaign Id</summary>
        public Guid? CampaignId { get; set; }

        /// <summary>Info</summary>
        public string Info { get; set; }

        /// <summary>Sender name</summary>
        public string SenderName { get; set; }

        /// <summary>Sender email</summary>
        public string SenderEmail { get; set; }

        /// <summary>Receiver name</summary>
        public string ReceiverName { get; set; }

        /// <summary>Receiver email</summary>
        public string ReceiverEmail { get; set; }

        /// <summary>Partner name</summary>
        public string PartnerName { get; set; }

        /// <summary>Location info</summary>
        public string LocationInfo { get; set; }

        /// <summary>Location external id</summary>
        public string LocationExternalId { get; set; }

        /// <summary>Location integration code</summary>
        public string LocationIntegrationCode { get; set; }

        /// <summary>Currency</summary>
        public string Currency { get; set; }
    }
}
