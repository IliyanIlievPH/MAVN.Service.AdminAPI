using System;
using Falcon.Numerics;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    public class TransactionsReportItem
    {
        public string Id { get; set; }
        
        public string SenderCustomerName { get; set; }
        
        public string SenderCustomerEmail { get; set; }
        
        public string SenderCustomerWallet { get; set; }
        
        public string InboundWalletAddress { get; set; }
        
        public DateTime? Timestamp { get; set; }
        
        public string TransactionType { get; set; }
        
        public string ActionRuleName { get; set; }
        
        public string ReceiverCustomerName { get; set; }
        
        public string ReceiverCustomerEmail { get; set; }
        
        public string ReceiverCustomerWallet { get; set; }
        
        public string OutboundWalletAddress { get; set; }
        
        public Money18? Amount { get; set; }
    }
}
