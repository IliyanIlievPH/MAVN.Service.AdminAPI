namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents image model
    /// </summary>
    public class ImageResponse
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
        public string RuleContentId { get; set; }

        /// <summary>
        /// Image blob url to show image on UI
        /// </summary>
        public string ImageBlobUrl { get; set; }
    }
}
