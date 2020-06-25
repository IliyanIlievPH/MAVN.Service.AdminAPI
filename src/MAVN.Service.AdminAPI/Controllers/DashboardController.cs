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
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Dashboard;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.DashboardStatistics.Client.Models.VoucherStatistic;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using Microsoft.AspNetCore.Mvc;
using CustomersStatisticResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomersStatisticResponse;
using VoucherDailyStatisticsModel = MAVN.Service.AdminAPI.Models.Dashboard.VoucherDailyStatisticsModel;
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
        private readonly ICurrencyConvertorClient _currencyConvertorClient;
        private readonly IMapper _mapper;
        private readonly IExtRequestContext _requestContext;
        private readonly ISettingsService _settingsService;

        public DashboardController(
            IDashboardStatisticsClient dashboardStatisticsClient,
            IPartnerManagementClient partnerManagementClient,
            ICurrencyConvertorClient currencyConvertorClient,
            IMapper mapper,
            IExtRequestContext requestContext,
            ISettingsService settingsService)
        {
            _dashboardStatisticsClient = dashboardStatisticsClient;
            _partnerManagementClient = partnerManagementClient;
            _currencyConvertorClient = currencyConvertorClient;
            _mapper = mapper;
            _requestContext = requestContext;
            _settingsService = settingsService;
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

            if (isEmptyResult)
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
        [ProducesResponseType(typeof(VoucherStatisticsResponse), (int)HttpStatusCode.OK)]
        public async Task<VoucherStatisticsResponse> GetSmartVouchersStatisticsAsync()
        {
            var (partnerIds, isEmptyResult) = await FilterByPartnerAsync();

            var result = new VoucherStatisticsResponse
            {
                Currency = _settingsService.GetBaseCurrency(),
            };

            if (isEmptyResult)
            {
                // Add some random data so only View admins will see something
                var rand = new Random();
                result.TotalPurchasesCost = rand.Next(1000);
                result.TotalRedeemedVouchersCost = rand.Next(1000);
                result.TotalPurchasesCount = rand.Next(100);
                result.TotalRedeemedVouchersCount = rand.Next(100);
                return result;
            }

            var statisticsForAllCurrencies = await _dashboardStatisticsClient.SmartVouchersApi.GetTotalStatisticsAsync(partnerIds);

            foreach (var statistics in statisticsForAllCurrencies)
            {
                if (!statistics.Currency.Equals(result.Currency, StringComparison.CurrentCultureIgnoreCase))
                {
                    var totalPurchaseCost = await _currencyConvertorClient.Converter.ConvertAsync(statistics.Currency.ToUpper(),
                        result.Currency.ToUpper(), statistics.TotalPurchasesCost);
                    var totalRedeemedCost = await _currencyConvertorClient.Converter.ConvertAsync(statistics.Currency.ToUpper(),
                        result.Currency.ToUpper(), statistics.TotalRedeemedVouchersCost);

                    result.TotalPurchasesCost += totalPurchaseCost.Amount;
                    result.TotalRedeemedVouchersCost += totalRedeemedCost.Amount;
                }
                else
                {
                    result.TotalPurchasesCost += statistics.TotalPurchasesCost;
                    result.TotalRedeemedVouchersCost += statistics.TotalRedeemedVouchersCost;
                }

                result.TotalPurchasesCount += statistics.TotalPurchasesCount;
                result.TotalRedeemedVouchersCount += statistics.TotalRedeemedVouchersCount;
            }

            return result;
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

            var result = new VoucherDailyStatisticsResponse();

            if (isEmptyResult)
                return result;

            var dailyStatistics = await _dashboardStatisticsClient.SmartVouchersApi.GetPeriodStatsAsync(new VouchersDailyStatisticsRequest
            {
                PartnerIds = partnerIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
            });

            var baseCurrency = _settingsService.GetBaseCurrency();

            result.BoughtVoucherStatistics = await CalculateStatisticsPerDayForBaseCurrency(dailyStatistics.BoughtVoucherStatistics, baseCurrency);
            result.UsedVoucherStatistics = await CalculateStatisticsPerDayForBaseCurrency(dailyStatistics.UsedVoucherStatistics, baseCurrency);

            return result;
        }

        private async Task<List<VoucherDailyStatisticsModel>> CalculateStatisticsPerDayForBaseCurrency
            (IEnumerable<DashboardStatistics.Client.Models.VoucherStatistic.VoucherDailyStatisticsModel> dailyStatistics, string baseCurrency)
        {
            var statisticsPerDay = new Dictionary<DateTime, VoucherDailyStatisticsModel>();

            foreach (var statistics in dailyStatistics)
            {
                decimal amount;
                if (!statistics.Currency.Equals(baseCurrency, StringComparison.CurrentCultureIgnoreCase))
                    amount = (await _currencyConvertorClient.Converter.ConvertAsync(statistics.Currency.ToUpper(),
                        baseCurrency.ToUpper(), statistics.Sum)).Amount;
                else
                    amount = statistics.Sum;

                if (statisticsPerDay.ContainsKey(statistics.Date))
                {
                    var current = statisticsPerDay[statistics.Date];
                    current.Sum += amount;
                    current.Count += statistics.Count;
                }
                else
                {
                    statisticsPerDay.Add(statistics.Date, new VoucherDailyStatisticsModel
                    {
                        Currency = baseCurrency,
                        Date = statistics.Date,
                        Count = statistics.Count,
                        Sum = amount
                    });
                }
            }

            return statisticsPerDay.Values.ToList();
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
