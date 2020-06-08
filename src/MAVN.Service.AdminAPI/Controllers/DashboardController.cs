using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.DashboardStatistics.Client;
using MAVN.Service.DashboardStatistics.Client.Models.Customers;
using MAVN.Service.DashboardStatistics.Client.Models.Tokens;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Dashboard;
using MAVN.Service.DashboardStatistics.Client.Models.VoucherStatistic;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using Microsoft.AspNetCore.Mvc;
using CustomersStatisticResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomersStatisticResponse;
using VoucherDailyStatisticsResponse = MAVN.Service.AdminAPI.Models.Dashboard.VoucherDailyStatisticsResponse;
using VoucherStatisticsResponse = MAVN.Service.AdminAPI.Models.Dashboard.VoucherStatisticsResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardStatisticsClient _dashboardStatisticsClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IMapper _mapper;
        private readonly IExtRequestContext _requestContext;

        public DashboardController(
            IDashboardStatisticsClient dashboardStatisticsClient,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper,
            IExtRequestContext requestContext
        )
        {
            _dashboardStatisticsClient = dashboardStatisticsClient;
            _partnerManagementClient = partnerManagementClient;
            _mapper = mapper;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Returns a statistics of customers.
        /// </summary>
        /// <returns>
        /// A statistics of customers.
        /// </returns>
        /// <response code="200">A statistics of customers.</response>
        [HttpGet("customers")]
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(CustomersStatisticResponseModel), (int)HttpStatusCode.OK)]
        public async Task<CustomersStatisticResponseModel> GetCustomerStatisticsAsync([FromQuery] CustomersListRequest request)
        {
            var (partnerIds, isEmptyResult) = await FilterByPartnerAsync();

            if(isEmptyResult)
                return new CustomersStatisticResponseModel();

            var result = await _dashboardStatisticsClient.CustomersApi.GetAsync(new CustomersListRequestModel
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                PartnerIds = partnerIds,
            });

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
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(TokensListResponse), (int)HttpStatusCode.OK)]
        public async Task<TokensListResponse> GetTokensStatisticsAsync([FromQuery] TokensListRequest request)
        {
            var result = await _dashboardStatisticsClient.TokensApi.GetAsync(_mapper.Map<TokensListRequestModel>(request));

            return _mapper.Map<TokensListResponse>(result);
        }

        /// <summary>
        /// Returns a statistics for smart vouchers.
        /// </summary>
        /// <returns>
        /// A statistics of tokens.
        /// </returns>
        /// <response code="200">A statistics for smart vouchers.</response>
        [HttpGet("smartvouchers/totals")]
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(List<VoucherStatisticsResponse>), (int)HttpStatusCode.OK)]
        public async Task<List<VoucherStatisticsResponse>> GetSmartVouchersStatisticsAsync()
        {
            var (partnerIds, isEmptyResult) = await FilterByPartnerAsync();

            if (isEmptyResult)
                return new List<VoucherStatisticsResponse>();

            var result = await _dashboardStatisticsClient.SmartVouchersApi.GetTotalStatisticsAsync(partnerIds);

            return _mapper.Map<List<VoucherStatisticsResponse>>(result);
        }

        /// <summary>
        /// Returns a statistics for smart vouchers for period.
        /// </summary>
        /// <returns>
        /// A statistics of tokens.
        /// </returns>
        /// <response code="200">A statistics of tokens.</response>
        [HttpGet("smartvouchers/period")]
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(VoucherDailyStatisticsResponse), (int)HttpStatusCode.OK)]
        public async Task<VoucherDailyStatisticsResponse> GetSmartVouchersStatisticsForPeriodAsync([FromQuery] BasePeriodRequest request)
        {
            var (partnerIds, isEmptyResult) = await FilterByPartnerAsync();

            if (isEmptyResult)
                return new VoucherDailyStatisticsResponse();

            var result = await _dashboardStatisticsClient.SmartVouchersApi.GetPeriodStatsAsync(new VouchersDailyStatisticsRequest
            {
                PartnerIds = partnerIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
            });

            return _mapper.Map<VoucherDailyStatisticsResponse>(result);
        }

        private async Task<(Guid[] PartnerIds, bool IsEmptyResult)> FilterByPartnerAsync()
        {
            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.Dashboard);

            if (!permissionLevel.HasValue || permissionLevel.Value == PermissionLevel.View) 
                return (null, true);

            var partnersResponse =
                await _partnerManagementClient.Partners.GetAsync(
                    new PartnerListRequestModel { CreatedBy = Guid.Parse(_requestContext.UserId), PageSize = 100, CurrentPage = 1 });

            if (partnersResponse.PartnersDetails.Count == 0)
                return (null, true);

            var partnerIds = partnersResponse.PartnersDetails
                .Select(x => x.Id)
                .ToArray();

            return (partnerIds, false);
        }
    }
}
