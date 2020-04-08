using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class TransactionListRequest
    {
        [Required]
        public PagedRequestModel PagedRequest { get; set; }

        public string FunctionName { get; set; }

        public string FunctionSignature { get; set; }

        public IEnumerable<string> From { get; set; }

        public IEnumerable<string> To { get; set; }

        public IEnumerable<string> AffectedAddresses { get; set; }
    }
}
