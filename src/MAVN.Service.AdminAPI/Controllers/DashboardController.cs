using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Service.DashboardStatistics.Client;
using Lykke.Service.DashboardStatistics.Client.Models.Customers;
using Lykke.Service.DashboardStatistics.Client.Models.Leads;
using Lykke.Service.DashboardStatistics.Client.Models.Tokens;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using CustomersStatisticResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomersStatisticResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.Dashboard, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardStatisticsClient _dashboardStatisticsClient;
        private readonly IMapper _mapper;

        public DashboardController(IDashboardStatisticsClient dashboardStatisticsClient, IMapper mapper)
        {
            _dashboardStatisticsClient = dashboardStatisticsClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a statistics of leads.
        /// </summary>
        /// <returns>
        /// A statistics of leads.
        /// </returns>
        /// <response code="200">A statistics of leads.</response>
        [HttpGet("leads")]
        [ProducesResponseType(typeof(LeadsListResponse), (int)HttpStatusCode.OK)]
        public async Task<LeadsListResponse> GetLeadsStatisticsAsync([FromQuery] LeadsListRequest request)
        {
            var result = await _dashboardStatisticsClient.LeadsApi.GetAsync(_mapper.Map<LeadsListRequestModel>(request));

            return _mapper.Map<LeadsListResponse>(result);
        }

        /// <summary>
        /// Returns a statistics of customers.
        /// </summary>
        /// <returns>
        /// A statistics of customers.
        /// </returns>
        /// <response code="200">A statistics of customers.</response>
        [HttpGet("customers")]
        [ProducesResponseType(typeof(CustomersStatisticResponseModel), (int)HttpStatusCode.OK)]
        public async Task<CustomersStatisticResponseModel> GetCustomerStatisticsAsync([FromQuery] CustomersListRequest request)
        {
            var result = await _dashboardStatisticsClient.CustomersApi.GetAsync(_mapper.Map<CustomersListRequestModel>(request));

            return _mapper.Map<CustomersStatisticResponseModel>(result);
        }

        /// <summary>
        /// Returns a statistics of tokens.
        /// </summary>
        /// <returns>
        /// A statistics of tokens.
        /// </returns>
        /// <response code="200">A statistics of tokens.</response>
        [HttpGet("tokens")]
        [ProducesResponseType(typeof(TokensListResponse), (int)HttpStatusCode.OK)]
        public async Task<TokensListResponse> GetTokensStatisticsAsync([FromQuery] TokensListRequest request)
        {
            var result = await _dashboardStatisticsClient.TokensApi.GetAsync(_mapper.Map<TokensListRequestModel>(request));

            return _mapper.Map<TokensListResponse>(result);
        }
    }
}
