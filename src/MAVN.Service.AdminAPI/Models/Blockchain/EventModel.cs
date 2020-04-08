using System;
using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class EventModel
    {
        public string BlockHash { get; set; }

        public long BlockNumber { get; set; }

        public string TransactionHash { get; set; }

        public long TransactionIndex { get; set; }

        public long LogIndex { get; set; }

        public string Address { get; set; }

        public string EventName { get; set; }

        public string EventSignature { get; set; }

        public IReadOnlyCollection<EventParameters> Parameters { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public class EventParameters
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
