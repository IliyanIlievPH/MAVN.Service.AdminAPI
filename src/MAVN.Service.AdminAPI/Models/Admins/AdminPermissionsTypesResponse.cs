using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents all admin permission types.
    /// </summary>
    public class AdminPermissionsTypesResponse
    {
        /// <summary>
        /// List of all possible admin permission types.
        /// </summary>
        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        public List<AdminPermissionType> Types { set; get; }
    }
}
