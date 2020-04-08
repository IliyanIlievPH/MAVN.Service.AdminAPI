using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Service.Reporting.Client;
using Lykke.Service.Reporting.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.Reports;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Permission(PermissionType.Reports, PermissionLevel.View)]
    [Route("/api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportClient _reportClient;
        private readonly IMapper _mapper;

        public ReportsController(IReportClient reportClient,
            IMapper mapper)
        {
            _reportClient = reportClient;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReportListModel), (int)HttpStatusCode.OK)]
        public async Task<ReportListModel> GetTransactionReportAsync([FromBody] ReportRequestModel request)
        {
            var toDate = request.To.Date.AddDays(1).AddMilliseconds(-1);
            var fromDate = request.From.Date;

            var clientResult = await _reportClient.Api.FetchReportAsync(new TransactionReportByTimeRequest()
            {
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize,
                From = fromDate,
                To = toDate
            });

            return new ReportListModel
            {
                Items = _mapper.Map<List<ReportItemModel>>(clientResult.TransactionReports),
                PagedResponse = new PagedResponseModel(request.CurrentPage, clientResult.TotalCount)
            };
        }

        [HttpGet("exportToCsv")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ExportTransactionReportAsync([FromQuery][Required] DateTime from, [FromQuery][Required]  DateTime to)
        {
            var toDate = to.Date.AddDays(1).AddMilliseconds(-1);
            var fromDate = from.Date;

            var clientResult = await _reportClient.Api.FetchReportCsvAsync(fromDate, toDate);

            var fileName = $"transactions_from_{@from:dd-MM-yyyy}_to_{to:dd-MM-yyyy}.csv";

            return clientResult.ToCsvFile(fileName);
        }
    }
}
