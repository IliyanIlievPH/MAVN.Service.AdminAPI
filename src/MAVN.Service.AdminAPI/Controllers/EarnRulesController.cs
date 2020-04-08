using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Common.Middleware.Authentication;
using Falcon.Numerics;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models;
using Lykke.Service.Campaign.Client.Models.Campaign.Responses;
using Lykke.Service.Campaign.Client.Models.EarnRuleContent;
using Lykke.Service.Campaign.Client.Models.Enums;
using Lykke.Service.Campaign.Client.Models.Files.Requests;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnerManagement.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Infrastructure.CustomFilters;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Models.Partners;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RewardType = Lykke.Service.Campaign.Client.Models.Enums.RewardType;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class EarnRulesController : ControllerBase
    {
        private const string ReferToRealEstateBonusType = "estate-lead-referral";
        private readonly ICampaignClient _campaignsClient;
        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;
        private readonly IRequestContext _requestContext;
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly IImageService _imageService;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly ILog _log;

        public EarnRulesController(
            ICampaignClient campaignClient,
            ISettingsService settingsService,
            IMapper mapper,
            IRequestContext requestContext,
            ICurrencyConvertorClient convertorClient,
            IImageService imageService,
            IPartnerManagementClient partnerManagementClient,
            ILogFactory logFactory)
        {
            _campaignsClient = campaignClient;
            _settingsService = settingsService;
            _mapper = mapper;
            _requestContext = requestContext;
            _currencyConverterClient = convertorClient;
            _imageService = imageService;
            _partnerManagementClient = partnerManagementClient;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>ca
        /// Get all earn rules.
        /// </summary>
        /// <returns>
        /// A collection of earn rules.
        /// </returns>
        /// <response code="200">A collection of earn rules.</response>
        /// <response code="400">An error occurred while getting earn rules.</response>
        [HttpGet]
        [ProducesResponseType(typeof(EarnRuleListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<EarnRuleListResponse> GetEarnRuleListAsync([FromQuery] EarnRuleListRequest model)
        {
            var response = await _campaignsClient.Campaigns.GetAsync(
                new Lykke.Service.Campaign.Client.Models.Campaign.Requests.CampaignsPaginationRequestModel
                {
                    PageSize = model.PageSize,
                    CurrentPage = model.CurrentPage,
                    CampaignName = model.EarnRuleName
                });

            var asset = _settingsService.GetTokenName();

            var globalCurrencyRate = await _currencyConverterClient.GlobalCurrencyRates.GetAsync();

            var campaignVerticals = new Dictionary<string, Vertical>();

            foreach (var camp in response.Campaigns)
            {
                var vertical = camp.Conditions.FirstOrDefault(c => !c.IsHiddenType)?.Vertical;

                if (vertical != null)
                    campaignVerticals.Add(camp.Id, vertical.Value);

                if (!camp.UsePartnerCurrencyRate)
                    continue;

                var ids = camp.Conditions.SelectMany(c => c.PartnerIds).Distinct().ToList();

                if (ids.Count == 1)
                {
                    var partner = await _partnerManagementClient.Partners.GetByIdAsync(ids.First());

                    if (partner != null && !partner.UseGlobalCurrencyRate)
                    {
                        camp.AmountInCurrency = partner.AmountInCurrency;
                        camp.AmountInTokens = partner.AmountInTokens;
                        continue;
                    }
                }

                camp.AmountInCurrency = globalCurrencyRate.AmountInCurrency;
                camp.AmountInTokens = globalCurrencyRate.AmountInTokens;

                camp.Reward = CalculateTotalReward(camp);
            }

            var campaigns = _mapper.Map<List<EarnRuleRowModel>>(response.Campaigns);

            foreach (var campaign in campaigns)
            {
                campaign.Asset = asset;

                if (campaignVerticals.TryGetValue(campaign.Id, out Vertical vertical))
                    campaign.Vertical = _mapper.Map<BusinessVertical>(vertical);
            }

            return new EarnRuleListResponse
            {
                EarnRules = campaigns,
                PagedResponse = new PagedResponseModel
                {
                    CurrentPage = response.CurrentPage,
                    TotalCount = response.TotalCount
                }
            };
        }

        /// <summary>
        /// Get earn rule by identifier.
        /// </summary>
        /// <param name="id">The identifier of earn rule.</param>
        /// <returns>
        /// An earn rule.
        /// </returns>
        /// <response code="200">An earn rule.</response>
        /// <response code="400">An error occurred while getting earn rule.</response>
        [HttpGet("query")]
        [ProducesResponseType(typeof(EarnRuleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<EarnRuleModel> GetEarnRuleByIdAsync([FromQuery] string id)
        {
            var campaign = await _campaignsClient.Campaigns.GetByIdAsync(id);

            ThrowIfError(campaign.ErrorCode, campaign.ErrorMessage);

            var asset = _settingsService.GetTokenName();

            // order conditions to have optional after first condition
            if (campaign.Conditions.Count > 0)
            {
                var orderedConditions = new List<Lykke.Service.Campaign.Client.Models.Condition.ConditionModel>();
                var specialCondition = campaign.Conditions.FirstOrDefault(x => x.Type.Equals(ReferToRealEstateBonusType));

                if (specialCondition != null)
                {
                    orderedConditions.Add(specialCondition);
                }

                orderedConditions.AddRange(campaign.Conditions.Where(x => !x.Type.Equals(ReferToRealEstateBonusType)));
            }

            var result = _mapper.Map<EarnRuleModel>(campaign);
            result.Asset = asset;

            // dictionary by Localization
            var mobileContentsDictionary = new Dictionary<Localization, MobileContentResponse>();

            foreach (var content in campaign.Contents)
            {
                if (mobileContentsDictionary.TryGetValue(content.Localization, out var existingMobileContent))
                {
                    FillMobileContent(existingMobileContent);
                }
                else
                {
                    var newMobileContent = new MobileContentResponse
                    {
                        MobileLanguage = content.Localization
                    };

                    FillMobileContent(newMobileContent);

                    mobileContentsDictionary.TryAdd(content.Localization, newMobileContent);
                }

                void FillMobileContent(MobileContentResponse mobileContent)
                {
                    switch (content.RuleContentType)
                    {
                        case RuleContentType.Title:
                            mobileContent.Title = content.Value;
                            mobileContent.TitleId = content.Id;
                            break;
                        case RuleContentType.Description:
                            mobileContent.Description = content.Value;
                            mobileContent.DescriptionId = content.Id;
                            break;
                        case RuleContentType.UrlForPicture:
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
        /// Creates a new earn rule.
        /// </summary>
        /// <returns>
        /// The information about the created earn rule.
        /// </returns>
        /// <response code="200">Response model with EarnRuleId.</response>
        /// <response code="400">An error occurred while creating earn rule.</response>
        [HttpPost]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType(typeof(EarnRuleCreatedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<EarnRuleCreatedResponse> CreateEarnRuleAsync([FromBody] EarnRuleCreateModel model)
        {
            var request = _mapper.Map<EarnRuleCreateModel,
                Lykke.Service.Campaign.Client.Models.Campaign.Requests.CampaignCreateModel>(model,
                opt => opt.AfterMap((src, dest) => { dest.CreatedBy = _requestContext.UserId; }));

            var mobileContents = new List<EarnRuleContentCreateRequest>();

            foreach (var mobileContent in model.MobileContents)
            {
                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new EarnRuleContentCreateRequest
                    {
                        Localization = mobileContent.MobileLanguage,
                        RuleContentType = RuleContentType.Title,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new EarnRuleContentCreateRequest
                {
                    Localization = mobileContent.MobileLanguage,
                    RuleContentType = RuleContentType.Description,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                // create content for adding image
                mobileContents.Add(new EarnRuleContentCreateRequest
                {
                    Localization = mobileContent.MobileLanguage,
                    RuleContentType = RuleContentType.UrlForPicture,
                    Value = null
                });
            }

            request.Contents = mobileContents;

            Lykke.Service.Campaign.Client.Models.Campaign.Responses.CampaignCreateResponseModel response;

            try
            {
                response = await _campaignsClient.Campaigns.CreateCampaignAsync(request);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);

            var createdCampaign = await _campaignsClient.Campaigns.GetByIdAsync(response.CampaignId);
            var createImageContents = new List<ImageContentCreatedResponse>();

            foreach (var content in createdCampaign.Contents)
            {
                if (content.RuleContentType == RuleContentType.UrlForPicture)
                {
                    createImageContents.Add(new ImageContentCreatedResponse
                    {
                        MobileLanguage = content.Localization,
                        RuleContentId = content.Id
                    });
                }
            }

            _log.Info($"New Earn Rule with id = '{response.CampaignId}' created by user with id = '{_requestContext.UserId}'",
                context: new
                {
                    _requestContext.UserId,
                    EarnRuleId = response.CampaignId,
                    Action = "CreateEarnRule"
                });

            return new EarnRuleCreatedResponse
            {
                Id = response.CampaignId,
                CreatedImageContents = createImageContents
            };
        }

        /// <summary>
        /// Adds an image to an earn rule.
        /// </summary>
        /// <param name="model">The image content.</param>
        /// <param name="formFile">The file</param>
        /// <response code="204">Image successfully added.</response>
        /// <response code="400">An error occurred while adding an image.</response>
        [HttpPost("image")]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task AddImage([FromQuery] ImageAddRequest model, [Required] IFormFile formFile)
        {
            var imageContent = _imageService.HandleFile(formFile, model.RuleContentId);

            var requestModel = _mapper.Map<ImageAddRequest, FileCreateRequest>(model,
                opt => opt.AfterMap((src, dest) =>
                {
                    dest.Type = formFile.ContentType;
                    dest.Name = formFile.FileName;
                    dest.Content = imageContent;
                }));

            CampaignServiceErrorResponseModel response;

            try
            {
                response = await _campaignsClient.Campaigns.AddImage(requestModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Updates an earn rule.
        /// </summary>
        /// <response code="204">An earn rule successfully updated.</response>
        /// <response code="400">An error occurred while updating an earn rule.</response>
        [HttpPut]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UpdateEarnRuleAsync([FromBody] EarnRuleUpdateModel model)
        {
            var request =
                _mapper.Map<EarnRuleUpdateModel, Lykke.Service.Campaign.Client.Models.Campaign.Requests.CampaignEditModel>(model);

            var mobileContents = new List<EarnRuleContentEditRequest>();

            foreach (var mobileContent in model.MobileContents)
            {
                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new EarnRuleContentEditRequest
                    {
                        Id = mobileContent.TitleId,
                        RuleContentType = RuleContentType.Title,
                        Localization = mobileContent.MobileLanguage,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new EarnRuleContentEditRequest
                {
                    Id = mobileContent.DescriptionId,
                    RuleContentType = RuleContentType.Description,
                    Localization = mobileContent.MobileLanguage,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                mobileContents.Add(new EarnRuleContentEditRequest
                {
                    Id = mobileContent.ImageId,
                    RuleContentType = RuleContentType.UrlForPicture,
                    Localization = mobileContent.MobileLanguage,
                    Value = mobileContent.ImageBlobUrl
                });
            }

            request.Contents = mobileContents;

            Lykke.Service.Campaign.Client.Models.Campaign.Responses.CampaignDetailResponseModel response;

            try
            {
                response = await _campaignsClient.Campaigns.UpdateAsync(request);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);

            _log.Info($"Earn Rule with id = '{model.Id}' updated by user with id = '{_requestContext.UserId}'",
                context: new
                {
                    _requestContext.UserId,
                    EarnRuleId = model.Id,
                    Action = "UpdateEarnRule"
                });
        }

        /// <summary>
        /// Updates image of an earn rule.
        /// </summary>
        /// <param name="model">The image content.</param>
        /// <param name="formFile">The file.</param>
        /// <response code="204">Image successfully updated.</response>
        /// <response code="400">An error occurred while updating an image.</response>
        [HttpPut("image")]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UpdateImage([FromQuery] ImageEditRequest model, [Required] IFormFile formFile)
        {
            var imageContent = _imageService.HandleFile(formFile, model.RuleContentId);

            var requestModel = _mapper.Map<ImageEditRequest, FileEditRequest>(model,
                opt => opt.AfterMap((src, dest) =>
                {
                    dest.Type = formFile.ContentType;
                    dest.Name = formFile.FileName;
                    dest.Content = imageContent;
                }));

            CampaignServiceErrorResponseModel response;

            try
            {
                response = await _campaignsClient.Campaigns.UpdateImage(requestModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Deletes Earn Rule.
        /// </summary>
        /// <param name="earnRuleId">The EarnRule identifier.</param>
        /// <response code="204">EarnRule successfully deleted.</response>
        /// <response code="400">An error occurred while deleting an earn rule.</response>
        [HttpDelete]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task DeleteEarnRuleAsync([FromQuery] string earnRuleId)
        {
            var response = await _campaignsClient.Campaigns.DeleteAsync(earnRuleId);

            ThrowIfError(response.ErrorCode, response.ErrorMessage);

            _log.Info($"Earn Rule with id = '{earnRuleId}' deleted by user with id = '{_requestContext.UserId}'",
                context: new
                {
                    _requestContext.UserId,
                    EarnRuleId = earnRuleId,
                    Action = "DeleteEarnRule"
                });
        }

        private static void ThrowIfError(CampaignServiceErrorCodes errorCode, string message)
        {
            if (errorCode != CampaignServiceErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(errorCode.ToString(), message));
        }
        
        private static Money18 CalculateTotalReward(CampaignResponse result)
        {
            if (result.RewardType == RewardType.Fixed && result.Conditions.Count > 1)
            {
                Money18 conditionReward = 0m; //todo: BO fix needed to stop copying condition details to campaign
                
                foreach (var condition in result.Conditions)
                {
                    conditionReward += condition.RewardType == RewardType.Fixed
                        ? condition.ImmediateReward
                        : condition.ApproximateAward ?? 0m;
                }

                return conditionReward;
            }
            else
            {
                return result.Reward;
            }
        }
    }
}
