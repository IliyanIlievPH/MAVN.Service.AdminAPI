namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    /// <summary>
    /// Represents mobile content model per language to show on UI
    /// </summary>
    public class MobileContentResponse : MobileContentEditRequest
    {
        /// <summary>
        /// Represents image model to show on UI
        /// </summary>
        public ImageResponse Image { get; set; } = new ImageResponse();
    }
}
