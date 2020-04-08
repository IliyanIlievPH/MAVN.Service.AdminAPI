using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents admin permission.
    /// </summary>
    [UsedImplicitly]
    public class AdminPermission
    {
        /// <summary>
        /// Level of permission.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AdminPermissionLevel Level { set; get; }
        
        /// <summary>
        /// Type of permission.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AdminPermissionType Type { set; get; }
    }
}
