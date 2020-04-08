using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents request model for mobile content creation
    /// </summary>
    public class MobileContentCreateRequest : IMobileContentRequest
    {
        /// <summary>
        /// Represents the language of the mobile content
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Localization MobileLanguage { get; set; }

        /// <summary>
        /// Represents the title of the mobile content
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents the description of the mobile content
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Image blob url to show image on UI
        /// </summary>
        public string ImageBlobUrl { get; set; }
    }
}
