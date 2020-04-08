using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class TransactionListResponse
    {
        public PagedResponseModel PagedResponse { get; set; }

        public IReadOnlyCollection<TransactionModel> Transactions { get; set; }
    }
}
