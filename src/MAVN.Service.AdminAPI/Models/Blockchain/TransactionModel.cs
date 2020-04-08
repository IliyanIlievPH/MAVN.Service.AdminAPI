using System.Collections.Generic;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class TransactionModel: BaseTransaction
    {
        public string ContractAddress { get; set; }

        public string FunctionName { get; set; }

        public string FunctionSignature { get; set; }

        public IReadOnlyCollection<EventModel> Events { get; set; }
    }
}
