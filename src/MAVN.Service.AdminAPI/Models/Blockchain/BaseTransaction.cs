using System;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public abstract class BaseTransaction
    {
        public string BlockHash { get; set; }

        public long BlockNumber { get; set; }

        public string TransactionHash { get; set; }

        public long TransactionIndex { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int Status { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
