using System;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents image edit request for a burn rule
    /// </summary>
    public class ImageEditRequest
    {
        /// <summary>
        /// Id of existing file
        /// Required for updating image
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Rule content id
        /// Required for adding and updating image
        /// </summary>
        public Guid RuleContentId { get; set; }
    }
}
