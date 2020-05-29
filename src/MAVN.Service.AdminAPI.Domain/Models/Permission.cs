using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    /// <summary>
    /// Represents admin permission.
    /// </summary>
    [UsedImplicitly]
    public class Permission
    {
        /// <summary>
        /// Level of permission.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PermissionLevel Level { set; get; }

        /// <summary>
        /// Type of permission.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PermissionType Type { set; get; }
    }
}
