using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses.Enums;
using Microsoft.AspNetCore.Mvc;
using PresentVouchersRequest = MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers.PresentVouchersRequest;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class SmartVouchersController : ControllerBase
    {
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IExtRequestContext _requestContext;
        private readonly IMapper _mapper;

        public SmartVouchersController(ISmartVouchersClient smartVouchersClient, IExtRequestContext requestContext, IMapper mapper)
        {
            _smartVouchersClient = smartVouchersClient;
            _requestContext = requestContext;
            _mapper = mapper;
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

        /// <summary>
        /// Present smart vouchers to customers
        /// </summary>
        /// <param name="request">Request model</param>
        [HttpPost("present")]
        [ProducesResponseType(typeof(PagedSmartVouchersListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<PresentSmartVouchersResponse> PresentVouchersAsync([FromBody] PresentVouchersRequest request)
        {
            var result = await _smartVouchersClient.VouchersApi.PresentVouchersAsync(
                new SmartVouchers.Client.Models.Requests.PresentVouchersRequest
                {
                    CampaignId = request.CampaignId,
                    CustomerEmails = request.CustomersEmails,
                    AdminId = Guid.Parse(_requestContext.UserId)
                });

            if (result.Error != PresentVouchersErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(result.Error.ToString()));

            return new PresentSmartVouchersResponse
            {
                NotRegisteredEmails = result.NotRegisteredEmails,
            };
        }
    }
}
