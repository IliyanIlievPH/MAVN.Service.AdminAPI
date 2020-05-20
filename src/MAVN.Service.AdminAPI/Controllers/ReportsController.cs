using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.Reporting.Client;
using MAVN.Service.Reporting.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.Reports;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Permission(
        PermissionType.Reports,
        new[]
        {
            PermissionLevel.View,
            PermissionLevel.PartnerEdit,
        }
    )]
    [Route("/api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportClient _reportClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IExtRequestContext _requestContext;
        private readonly IMapper _mapper;

        public ReportsController(
            IReportClient reportClient,
            IPartnerManagementClient partnerManagementClient,
            IExtRequestContext requestContext,
            IMapper mapper)
        {
            _reportClient = reportClient;
            _partnerManagementClient = partnerManagementClient;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReportListModel), (int)HttpStatusCode.OK)]
        public async Task<ReportListModel> GetTransactionReportAsync([FromBody] ReportRequestModel request)
        {
            var filter = await FilterByPartnerAsync(request.PartnerId);

            if (filter.IsEmptyResult)
            {
                return new ReportListModel
                {
                    Items = new List<ReportItemModel>(),
                    PagedResponse = new PagedResponseModel(request.CurrentPage, 0)
                };
            }

            var requestModel = new TransactionReportByTimeRequest()
            {
                From = request.From.Date,
                To = request.To.Date.AddDays(1).AddMilliseconds(-1),
                TransactionType = request.TransactionType,
                Status = request.Status,
            };
            var clientResult = await _reportClient.Api.FetchReportAsync(requestModel, filter.PartnerIds);

            return new ReportListModel
            {
                Items = _mapper.Map<List<ReportItemModel>>(clientResult.TransactionReports),
                PagedResponse = new PagedResponseModel(request.CurrentPage, clientResult.TotalCount)
            };
        }

        [HttpGet("exportToCsv")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ExportTransactionReportAsync([FromQuery][Required] DateTime from, [FromQuery][Required] DateTime to, [FromQuery] Guid partnerId, [FromQuery] string transactionType, [FromQuery] string status)
        {
            var fileName = $"transactions_from_{from:dd-MM-yyyy}_to_{to:dd-MM-yyyy}.csv";
            var filter = await FilterByPartnerAsync(partnerId);

            if (filter.IsEmptyResult)
            {
                return new FileContentResult(new byte[0], "text/csv")
                {
                    FileDownloadName = fileName
                };
            }

            var requestModel = new TransactionReportByTimeRequest()
            {
                From = from.Date,
                To = to.Date.AddDays(1).AddMilliseconds(-1),
                TransactionType = transactionType,
                Status = status,
            };
            var clientResult = await _reportClient.Api.FetchReportCsvAsync(requestModel, filter.PartnerIds);

            return clientResult.ToCsvFile(fileName);
        }

        private async Task<(string[] PartnerIds, bool IsEmptyResult)> FilterByPartnerAsync(Guid? partnerId)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.Reports);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                var partnersResponse =
                    await _partnerManagementClient.Partners.GetAsync(
                        new PartnerListRequestModel { CreatedBy = Guid.Parse(_requestContext.UserId), PageSize = 100, CurrentPage = 1 });

                if (partnersResponse.PartnersDetails.Count == 0)
                {
                    return (null, true);
                }

                var partnerIds = partnersResponse.PartnersDetails
                    .Where(x => partnerId.HasValue ? partnerId.Value.Equals(x.Id) : true)
                    .Select(x => x.Id.ToString())
                    .ToArray();

                return (partnerIds, false);
            }

            return (partnerId.HasValue ? new string[] { partnerId.Value.ToString() } : null, false);

            #endregion
        }
    }
}
