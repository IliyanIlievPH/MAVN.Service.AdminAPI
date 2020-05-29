using System;
using MAVN.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    public class CustomerOperationModel
    {
        /// <summary>
        /// Date and time of the operation
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Internal id of the trasnaction
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Type of the transaction. Values can be: P2P, Earn, Burn
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CustomerOperationTransactionType TransactionType { get; set; }

        /// <summary>
        /// Name of the campaign (only for Earn and Burn)
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Blockchain address of the wallet (only for P2P)
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// Id of the receiver (only for P2P)
        /// </summary>
        public string ReceiverCustomerId { get; set; }

        /// <summary>
        /// Id of the partner (only for Earn and Burn)
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Received amount
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Asset symbol
        /// </summary>
        public string AssetSymbol { get; set; }

    }
}
