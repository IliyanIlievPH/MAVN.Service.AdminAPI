using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Job.TokensStatistics.Client;
using Lykke.Job.TokensStatistics.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client;
using Lykke.Service.OperationsHistory.Client;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.Referral.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Infrastructure.Extensions;
using MAVN.Service.AdminAPI.Models.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    // No permission on controller because there is one method which should be accessible
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private static readonly TimeZoneInfo AstTimeZone;

        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly ITokensStatisticsClient _tokensStatisticsClient;
        private readonly IReferralClient _referralClient;
        private readonly IPrivateBlockchainFacadeClient _privateBlockchainFacadeClient;
        private readonly IMapper _mapper;

        static StatisticsController()
        {
            AstTimeZone = TimeZoneInfo.GetSystemTimeZones()
                .First(timeZone => timeZone.Id == "Asia/Dubai" || timeZone.Id == "Arabian Standard Time");
        }

        public StatisticsController(
            ICustomerProfileClient customerProfileClient,
            IOperationsHistoryClient operationsHistoryClient,
            ITokensStatisticsClient tokensStatisticsClient,
            IReferralClient referralClient,
            IPrivateBlockchainFacadeClient privateBlockchainFacadeClient,
            IMapper mapper)
        {
            _operationsHistoryClient = operationsHistoryClient;
            _tokensStatisticsClient = tokensStatisticsClient;
            _referralClient = referralClient;
            _customerProfileClient = customerProfileClient;
            _privateBlockchainFacadeClient = privateBlockchainFacadeClient;
            _mapper = mapper;
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
        [ProducesResponseType(typeof(CustomersStatisticsModel), (int)HttpStatusCode.OK)]
        public async Task<CustomersStatisticsModel> GetByCustomersAsync()
        {
            var date = TimeZoneInfo.ConvertTime(DateTime.UtcNow, AstTimeZone).Date;

            var registrationStartDate = TimeZoneInfo.ConvertTimeToUtc(date.AddDays(-1), AstTimeZone);
            var activityStartDate = TimeZoneInfo.ConvertTimeToUtc(date.AddDays(-30), AstTimeZone);
            var endDate = TimeZoneInfo.ConvertTimeToUtc(date.AddSeconds(-1), AstTimeZone);

            var registeredTask = _customerProfileClient.Statistics
                .GetByPeriodAsync(registrationStartDate, endDate);

            var activeTask = _operationsHistoryClient.StatisticsApi
                .GetActiveCustomersCountAsync(activityStartDate, endDate);

            await Task.WhenAll(registeredTask, activeTask);

            var model = new CustomersStatisticsModel
            {
                ActiveCount = activeTask.Result.ActiveCustomersCount,
                RegistrationsCount = registeredTask.Result.RegistrationsCount,
                TotalCount = registeredTask.Result.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Returns a statistics of tokens.
        /// </summary>
        /// <returns>
        /// A statistics of tokens.
        /// </returns>
        /// <response code="200">A statistics of tokens.</response>
        [HttpGet("tokens-current")]
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(TokensStatisticsModel), (int)HttpStatusCode.OK)]
        public async Task<TokensStatisticsModel> GetByTokensCurrentAsync()
        {
            var totalTokens = await _tokensStatisticsClient.Api.GetByDaysAsync(new PeriodRequest
            {
                FromDate = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc), // to have total tokens
                ToDate = DateTime.UtcNow
            });

            var model = new TokensStatisticsModel
            {
                EarnedCount = totalTokens.TotalEarn,
                BurnedCount = totalTokens.TotalBurn,
                TotalCount = totalTokens.TotalCustomersWalletBalance
            };

            return model;
        }

        /// <summary>
        /// Returns a statistics of Real Estate Leads.
        /// </summary>
        /// <returns>
        /// A statistics of leads.
        /// </returns>
        /// <response code="200">A statistics of leads.</response>
        [HttpGet("leads")]
        [Permission(PermissionType.Dashboard, PermissionLevel.View)]
        [ProducesResponseType(typeof(LeadStatisticModel), (int)HttpStatusCode.OK)]
        public async Task<LeadStatisticModel> GetByLeadsAsync()
        {
            var leadStatistic = await _referralClient.ReferralLeadApi.GetLeadStatisticAsync();

            var model = _mapper.Map<LeadStatisticModel>(leadStatistic);

            return model;
        }

        /// <summary>
        /// Returns a total supply of SPs.
        /// </summary>
        /// <returns>
        /// Total supply .
        /// </returns>
        /// <response code="200">A total supply of SPs.</response>
        [HttpGet("total-supply")]
        [Permission(
            PermissionType.Dashboard,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(TokenSupplyModel), (int)HttpStatusCode.OK)]
        public async Task<TokenSupplyModel> GetTotalSupplyAsync()
        {
            var totalSupply = await _privateBlockchainFacadeClient.TokensApi.GetTotalTokensSupplyAsync();

            var decimalVal = (decimal) totalSupply.TotalTokensAmount;

            return new TokenSupplyModel
            {
                TotalSupply = decimalVal.ToKBM()
            };
        }
    }
}
