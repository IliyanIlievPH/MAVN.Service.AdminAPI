using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Blockchain
{
    public class EventListResponse
    {
        public PagedResponseModel PagedResponse { get; set; }

        public IReadOnlyCollection<EventModel> Events { get; set; }
    }
}
