using System;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Locations.Requests
{
    [PublicAPI]
    public class LocationEditRequest : LocationModel
    {
        public Guid? Id { get; set; }
    }
}
