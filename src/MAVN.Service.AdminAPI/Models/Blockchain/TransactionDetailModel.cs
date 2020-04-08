using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class TransactionDetailModel: BaseTransaction
    {
        public string Input { get; set; }

        public IReadOnlyCollection<TransactionLog> Logs { get; set; }

        public FunctionCall FunctionCall { get; set; }

        public IReadOnlyCollection<EventModel> Events { get; set; }
    }

    public class TransactionLog
    {
        public string Address { get; set; }

        public long LogIndex { get; set; }

        public string Data { get; set; }

        public string[] Topics { get; set; }
    }

    public class FunctionCall
    {
        public string FunctionName { get; set; }

        public string FunctionSignature { get; set; }

        public IReadOnlyCollection<EventParameters> Parameters { get; set; }
    }
}
