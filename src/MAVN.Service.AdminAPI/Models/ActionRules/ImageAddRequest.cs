using System;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents image add request for a burn rule
    /// </summary>
    public class ImageAddRequest
    {
        /// <summary>
        /// Rule content ids
        /// Required for adding and updating image
        /// </summary>
        public Guid RuleContentId { get; set; }
    }
}
