using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class EventListRequest
    {
        [Required]
        public PagedRequestModel PagedRequest { get; set; }

        public string EventName { get; set; }

        public string EventSignature { get; set; }

        public string Address { get; set; }

        public IEnumerable<string> AffectedAddresses { get; set; }
    }
}
