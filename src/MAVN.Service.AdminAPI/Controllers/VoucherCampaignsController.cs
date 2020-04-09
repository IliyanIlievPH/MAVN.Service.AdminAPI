using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses;
using MAVN.Service.SmartVouchers.Client.Models.Responses.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class VoucherCampaignsController : ControllerBase
    {
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public VoucherCampaignsController(ISmartVouchersClient smartVouchersClient, IRequestContext requestContext, IMapper mapper)
        {
            _smartVouchersClient = smartVouchersClient;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        /// <summary>ca
        /// Get all smart voucher campaigns.
        /// </summary>
        /// <returns>
        /// A collection of smart voucher campaigns.
        /// </returns>
        /// <response code="200">A collection of smart voucher campaigns.</response>
        /// <response code="400">An error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedSmartVoucherCampaignsListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PaginatedSmartVoucherCampaignsListResponse> GetSmartVoucherCampaignsListAsync([FromQuery] SmartVoucherCampaignsListRequest request)
        {
            var result = await _smartVouchersClient.CampaignsApi.GetAsync(new VoucherCampaignsPaginationRequestModel
            {
                CampaignName = request.CampaignName,
                CurrentPage = request.CurrentPage,
                OnlyActive = request.OnlyActive,
                PageSize = request.PageSize
            });

            return new PaginatedSmartVoucherCampaignsListResponse
            {
                PagedResponse = new PagedResponseModel(request.CurrentPage, result.TotalCount),
                SmartVoucherCampaigns = _mapper.Map<List<SmartVoucherCampaignResponse>>(result.Campaigns)
            };
        }

        /// <summary>ca
        /// Get smart voucher campaign by Id.
        /// </summary>
        /// <returns>
        /// A smart voucher campaign details.
        /// </returns>
        /// <response code="200">A smart voucher campaign.</response>
        /// <response code="400">An error occurred.</response>
        [HttpGet("{campaignId}")]
        [ProducesResponseType(typeof(SmartVoucherCampaignDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<SmartVoucherCampaignDetailsResponse> GetByIdAsync(Guid campaignId)
        {
            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(campaignId);

            if (campaign == null)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode("SmartVoucherCampaignDoesNotExist",
                    "Smart voucher campaign with this id does not exist"));

            var result = _mapper.Map<SmartVoucherCampaignDetailsResponse>(campaign);

            return result;
        }

        /// <summary>
        /// Create smart voucher campaign
        /// </summary>
        /// <returns>
        /// Campaign ID
        /// </returns>
        /// <response code="200">Created campaign id.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public Task<Guid> CreateAsync([FromBody] SmartVoucherCampaignCreateRequest request)
        {
            var campaign = _mapper.Map<VoucherCampaignCreateModel>(request);
            campaign.CreatedBy = _requestContext.UserId;

            return _smartVouchersClient.CampaignsApi.CreateAsync(campaign);
        }

        /// <summary>
        /// Create smart voucher campaign
        /// </summary>
        /// <returns>
        /// Campaign ID
        /// </returns>
        /// <response code="200">Created campaign id.</response>
        [HttpPut]
        [ProducesResponseType(typeof(UpdateVoucherCampaignErrorCodes), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        public async Task UpdateAsync([FromBody] SmartVoucherCampaignEditRequest request)
        {
            var result = await _smartVouchersClient.CampaignsApi.UpdateAsync(_mapper.Map<VoucherCampaignEditModel>(request));

            if (result != UpdateVoucherCampaignErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(result.ToString()));
        }

        /// <summary>
        /// Delete smart voucher campaign
        /// </summary>
        /// <returns>
        /// </returns>
        /// <response code="204">Delete campaign id.</response>
        [HttpDelete("{campaignId}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task DeleteAsync(Guid campaignId)
        {
            var result = await _smartVouchersClient.CampaignsApi.DeleteAsync(campaignId);

            if (result == false)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode("Campaign was not deleted"));
        }

        /// <summary>
        /// Set image
        /// </summary>
        /// <returns>
        /// </returns>
        /// <response code="204">Image set successfully.</response>
        /// <response code="400">Bad request.</response>
        [HttpPost("image")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task SetImage([FromBody] SmartVoucherCampaignSetImageRequest request)
        {
            var imageModel = _mapper.Map<CampaignImageFileRequest>(request);

            var error = await _smartVouchersClient.CampaignsApi.SetImage(imageModel);

            if(error != SaveImageErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
        }
    }
}
