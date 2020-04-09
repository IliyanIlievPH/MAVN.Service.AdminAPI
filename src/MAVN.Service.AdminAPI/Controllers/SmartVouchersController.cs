using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class SmartVouchersController : ControllerBase
    {
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IMapper _mapper;

        public SmartVouchersController(ISmartVouchersClient smartVouchersClient, IMapper mapper)
        {
            _smartVouchersClient = smartVouchersClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get voucher details by its short code.
        /// </summary>
        /// <param name="voucherShortCode"></param>
        [HttpGet("{voucherShortCode}")]
        [ProducesResponseType(typeof(SmartVoucherDetailsResponse), (int)HttpStatusCode.OK)]
        public async Task<SmartVoucherDetailsResponse> GetByShortCodeAsync(string voucherShortCode)
        {
            var result = await _smartVouchersClient.VouchersApi.GetByShortCodeAsync(voucherShortCode);

            return _mapper.Map<SmartVoucherDetailsResponse>(result);
        }

        /// <summary>
        /// Get smart vouchers for specified voucher campaign.
        /// </summary>
        /// <param name="campaignId">Voucher campaign id.</param>
        /// <param name="pageData">Page data.</param>
        [HttpGet("bycampaign")]
        [ProducesResponseType(typeof(PagedSmartVouchersListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<PagedSmartVouchersListResponse> GetCampaignVouchersAsync(Guid campaignId, [FromQuery] PagedRequestModel pageData)
        {
            var pageInfo = _mapper.Map<BasePaginationRequestModel>(pageData);
            var result = await _smartVouchersClient.VouchersApi.GetCampaignVouchersAsync(campaignId, pageInfo);

            return new PagedSmartVouchersListResponse
            {
                PagedResponse = new PagedResponseModel(pageInfo.CurrentPage,result.TotalCount),
                Vouchers = _mapper.Map<List<SmartVoucherResponse>>(result.Vouchers)
            };
        }

        /// <summary>
        /// Get smart vouchers for specified customer.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <param name="pageData">Page data.</param>
        [HttpGet("bycustomer")]
        [ProducesResponseType(typeof(PagedSmartVouchersListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<PagedSmartVouchersListResponse> GetCustomerVouchersAsync(Guid customerId, [FromQuery] PagedRequestModel pageData)
        {
            var pageInfo = _mapper.Map<BasePaginationRequestModel>(pageData);
            var result = await _smartVouchersClient.VouchersApi.GetCustomerVouchersAsync(customerId, pageInfo);

            return new PagedSmartVouchersListResponse
            {
                PagedResponse = new PagedResponseModel(pageInfo.CurrentPage, result.TotalCount),
                Vouchers = _mapper.Map<List<SmartVoucherResponse>>(result.Vouchers)
            };
        }
    }
}
