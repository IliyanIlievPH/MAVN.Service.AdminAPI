using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents response model for image created for a burn rule
    /// </summary>
    public class ImageContentCreatedResponse
    {
        /// <summary>
        /// Represents the mobile language of the created image
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MobileLocalization MobileLanguage { get; set; }

        /// <summary>
        /// Represents the rule content id of the created image
        /// </summary>
        public Guid RuleContentId { get; set; }
    }
}
