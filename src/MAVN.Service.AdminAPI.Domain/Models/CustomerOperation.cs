using System;
using Falcon.Numerics;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    public class CustomerOperation
    {
        public DateTime Timestamp { get; set; }
        
        public string TransactionId { get; set; }
        
        public CustomerOperationTransactionType TransactionType { get; set; }
        
        public string CampaignName { get; set; }
        
        public string WalletAddress { get; set; }
        
        public string ReceiverCustomerId { get; set; }
        
        public string PartnerId { get; set; }
        
        public Money18 Amount { get; set; }
        
        public string AssetSymbol { get; set; }
    }
}
