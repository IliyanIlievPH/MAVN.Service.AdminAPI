using System;
using Lykke.Service.Campaign.Client.Models.Enums;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    public class MobileContentEditRequest : MobileContentCreateRequest
    {
        /// <summary>
        /// Rule content identifier of Title
        /// <see cref="RuleContentType"/>
        /// </summary>
        public Guid TitleId { get; set; }

        /// <summary>
        /// Rule content identifier of Description
        /// <see cref="RuleContentType"/>
        /// </summary>
        public Guid DescriptionId { get; set; }

        /// <summary>
        /// Rule content identifier of Image
        /// <see cref="RuleContentType"/>
        /// </summary>
        public Guid ImageId { get; set; }
    }
}
