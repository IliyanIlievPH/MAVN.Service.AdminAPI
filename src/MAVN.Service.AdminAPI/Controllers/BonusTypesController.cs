using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Service.Campaign.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.BonusTypes;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class BonusTypesController : Controller
    {
        private readonly ICampaignClient _campaignClient;
        private readonly IMapper _mapper;

        public BonusTypesController(ICampaignClient campaignClient, IMapper mapper)
        {
            _campaignClient = campaignClient ?? throw new ArgumentNullException(nameof(campaignClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get all bonus types.
        /// </summary>
        /// <remarks>
        /// Returns available bonus types.
        /// </remarks>
        /// <returns>
        /// A collection of bonus types.
        /// </returns>
        /// <response code="200">A collection of bonus types.</response>
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<BonusTypeModel>> GetAllBonusTypesAsync()
        {
            var response = await _campaignClient.BonusTypes.GetActiveAsync();

            response.BonusTypes = response.BonusTypes.OrderBy(x => x.Order).ToList();

            var result = _mapper.Map<BonusTypeModel[]>(response.BonusTypes);

            return result;
        }
    }
}
