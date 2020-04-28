using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Enums;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublishedAndActiveCampaignsVouchersCountResponse = MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns.PublishedAndActiveCampaignsVouchersCountResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(
        PermissionType.VoucherManager,
        new[]
        {
            PermissionLevel.View,
            PermissionLevel.PartnerEdit,
        }
    )]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class VoucherCampaignsController : ControllerBase
    {
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IExtRequestContext _requestContext;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public VoucherCampaignsController(
            ISmartVouchersClient smartVouchersClient,
            IExtRequestContext requestContext,
            IImageService imageService,
            IMapper mapper)
        {
            _smartVouchersClient = smartVouchersClient;
            _requestContext = requestContext;
            _imageService = imageService;
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
            var model = new VoucherCampaignsPaginationRequestModel
            {
                CampaignName = request.CampaignName,
                CurrentPage = request.CurrentPage,
                OnlyActive = request.OnlyActive,
                PageSize = request.PageSize
            };

            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // TODO: filter data for current _requestContext.UserId
            }

            #endregion

            var result = await _smartVouchersClient.CampaignsApi.GetAsync(model);

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
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<SmartVoucherCampaignDetailsResponse> GetByIdAsync(Guid campaignId)
        {
            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(campaignId);

            if (campaign == null)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode("SmartVoucherCampaignDoesNotExist",
                    "Smart voucher campaign with this id does not exist"));

            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // filter data for current _requestContext.UserId
                if (campaign.CreatedBy != _requestContext.UserId)
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var result = _mapper.Map<SmartVoucherCampaignDetailsResponse>(campaign);

            // dictionary by Localization
            var mobileContentsDictionary = new Dictionary<string, MobileContentResponse>();

            foreach (var content in campaign.LocalizedContents)
            {
                if (mobileContentsDictionary.TryGetValue(content.Localization.ToString(), out var existingMobileContent))
                {
                    FillMobileContent(existingMobileContent);
                }
                else
                {
                    Enum.TryParse<MobileLocalization>(content.Localization.ToString(), out var mobileLanguage);

                    var newMobileContent = new MobileContentResponse { MobileLanguage = mobileLanguage };

                    FillMobileContent(newMobileContent);

                    mobileContentsDictionary.TryAdd(content.Localization.ToString(), newMobileContent);
                }

                void FillMobileContent(MobileContentResponse mobileContent)
                {
                    switch (content.ContentType)
                    {
                        case VoucherCampaignContentType.Name:
                            mobileContent.Title = content.Value;
                            mobileContent.TitleId = content.Id;
                            break;
                        case VoucherCampaignContentType.Description:
                            mobileContent.Description = content.Value;
                            mobileContent.DescriptionId = content.Id;
                            break;
                        case VoucherCampaignContentType.ImageUrl:
                            mobileContent.ImageId = content.Id;
                            mobileContent.ImageBlobUrl = content.Value;
                            mobileContent.Image = new ImageResponse
                            {
                                Id = content.Image?.Id,
                                RuleContentId = content.Id.ToString(),
                                ImageBlobUrl = content.Image?.BlobUrl
                            };
                            break;
                    }
                }
            }

            result.MobileContents = mobileContentsDictionary.ToList().OrderBy(x => x.Key).Select(x => x.Value).ToList();

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
        [Permission(
            PermissionType.VoucherManager,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<SmartVoucherCampaignCreatedResponse> CreateAsync([FromBody] SmartVoucherCampaignCreateRequest model)
        {
            var campaign = _mapper.Map<VoucherCampaignCreateModel>(model);
            campaign.CreatedBy = _requestContext.UserId;

            var mobileContents = new List<VoucherCampaignContentCreateModel>();

            foreach (var mobileContent in model.MobileContents)
            {
                Enum.TryParse<SmartVouchers.Client.Models.Enums.Localization>(mobileContent.MobileLanguage.ToString(), out var mobileLanguage);

                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new VoucherCampaignContentCreateModel
                    {
                        Localization = mobileLanguage,
                        ContentType = VoucherCampaignContentType.Name,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new VoucherCampaignContentCreateModel
                {
                    Localization = mobileLanguage,
                    ContentType = VoucherCampaignContentType.Description,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                // create content for adding image
                mobileContents.Add(new VoucherCampaignContentCreateModel
                {
                    Localization = mobileLanguage,
                    ContentType = VoucherCampaignContentType.ImageUrl,
                    Value = "#"
                });
            }

            campaign.LocalizedContents = mobileContents;

            Guid campaignId;

            try
            {
                campaignId = await _smartVouchersClient.CampaignsApi.CreateAsync(campaign);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            var createdCampaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(campaignId);
            var createImageContents = new List<ImageContentCreatedResponse>();

            foreach (var content in createdCampaign.LocalizedContents)
            {
                if (content.ContentType == VoucherCampaignContentType.ImageUrl)
                {
                    Enum.TryParse<MobileLocalization>(content.Localization.ToString(), out var mobileLanguage);

                    createImageContents.Add(new ImageContentCreatedResponse
                    {
                        MobileLanguage = mobileLanguage,
                        RuleContentId = content.Id
                    });
                }
            }

            return new SmartVoucherCampaignCreatedResponse { Id = campaignId, CreatedImageContents = createImageContents };
        }

        /// <summary>
        /// Update smart voucher campaign
        /// </summary>
        /// <returns>
        /// </returns>
        /// <response code="204"></response>
        [HttpPut]
        [Permission(
            PermissionType.VoucherManager,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(UpdateVoucherCampaignErrorCodes), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        public async Task UpdateAsync([FromBody] SmartVoucherCampaignEditRequest model)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                var existingCampaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(model.Id);

                // filter data for current _requestContext.UserId
                if (existingCampaign != null &&
                    existingCampaign.CreatedBy != _requestContext.UserId)
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var campaign = _mapper.Map<VoucherCampaignEditModel>(model);

            var mobileContents = new List<VoucherCampaignContentEditModel>();

            foreach (var mobileContent in model.MobileContents)
            {
                Enum.TryParse<SmartVouchers.Client.Models.Enums.Localization>(mobileContent.MobileLanguage.ToString(), out var mobileLanguage);

                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new VoucherCampaignContentEditModel
                    {
                        Id = mobileContent.TitleId,
                        ContentType = VoucherCampaignContentType.Name,
                        Localization = mobileLanguage,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new VoucherCampaignContentEditModel
                {
                    Id = mobileContent.DescriptionId,
                    ContentType = VoucherCampaignContentType.Description,
                    Localization = mobileLanguage,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                mobileContents.Add(new VoucherCampaignContentEditModel
                {
                    Id = mobileContent.ImageId,
                    ContentType = VoucherCampaignContentType.ImageUrl,
                    Localization = mobileLanguage,
                    Value = mobileContent.ImageBlobUrl
                });
            }

            campaign.LocalizedContents = mobileContents;

            UpdateVoucherCampaignErrorCodes response;

            try
            {
                response = await _smartVouchersClient.CampaignsApi.UpdateAsync(campaign);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            if (response != UpdateVoucherCampaignErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(response.ToString()));
        }

        /// <summary>
        /// Delete smart voucher campaign
        /// </summary>
        /// <returns>
        /// </returns>
        /// <response code="204">Delete campaign id.</response>
        [HttpDelete("{campaignId}")]
        [Permission(
            PermissionType.VoucherManager,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task DeleteAsync(Guid campaignId)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // TODO: send _requestContext.UserId
            }

            #endregion

            var result = await _smartVouchersClient.CampaignsApi.DeleteAsync(campaignId);

            if (result == false)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode("Campaign was not deleted"));
        }

        /// <summary>
        /// Set image
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="model">The image content fields.</param>
        /// <param name="formFile">The file.</param>
        /// <response code="204">Image set successfully.</response>
        /// <response code="400">Bad request.</response>
        [HttpPost("image")]
        [Permission(
            PermissionType.VoucherManager,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task SetImage([FromQuery] SmartVoucherCampaignSetImageRequest model, [Required] IFormFile formFile)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                var existingCampaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(model.CampaignId);

                // filter data for current _requestContext.UserId
                if (existingCampaign != null &&
                    existingCampaign.CreatedBy != _requestContext.UserId)
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var imageContent = _imageService.HandleFile(formFile, model.ContentId);

            var imageModel = _mapper.Map<SmartVoucherCampaignSetImageRequest, CampaignImageFileRequest>(model,
                opt => opt.AfterMap((src, dest) =>
                {
                    dest.Type = formFile.ContentType;
                    dest.Name = formFile.FileName;
                    dest.Content = imageContent;
                }));

            SaveImageErrorCodes error;

            try
            {
                error = await _smartVouchersClient.CampaignsApi.SetImage(imageModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            if (error != SaveImageErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
        }

        /// <summary>
        /// Get total vouchers count for active and published campaigns
        /// </summary>
        /// <returns>
        /// </returns>
        /// <response code="200">Total count</response>
        [HttpGet("totalsupply")]
        [ProducesResponseType(typeof(PublishedAndActiveCampaignsVouchersCountResponse), (int)HttpStatusCode.OK)]
        public async Task<PublishedAndActiveCampaignsVouchersCountResponse> GetPublishedAndActiveCampaignsVouchersCountAsync()
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.VoucherManager);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // TODO: send _requestContext.UserId
            }

            #endregion

            var result = await _smartVouchersClient.CampaignsApi.GetPublishedAndActiveCampaignsVouchersCountAsync();

            return new PublishedAndActiveCampaignsVouchersCountResponse
            {
                ActiveCampaignsVouchersTotalCount = result.ActiveCampaignsVouchersTotalCount,
                PublishedCampaignsVouchersTotalCount = result.PublishedCampaignsVouchersTotalCount
            };

        }
    }
}
