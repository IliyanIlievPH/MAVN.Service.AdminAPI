using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models;
using Lykke.Service.Campaign.Client.Models.BurnRule.Requests;
using Lykke.Service.Campaign.Client.Models.BurnRule.Responses;
using Lykke.Service.Campaign.Client.Models.BurnRuleContent;
using Lykke.Service.Campaign.Client.Models.Enums;
using Lykke.Service.Campaign.Client.Models.Files.Requests;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnerManagement.Client.Models;
using Lykke.Service.PartnerManagement.Client.Models.Partner;
using Lykke.Service.Vouchers.Client;
using Lykke.Service.Vouchers.Client.Models;
using Lykke.Service.Vouchers.Client.Models.Vouchers;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Infrastructure.CustomFilters;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using BurnRuleCreateRequest = MAVN.Service.AdminAPI.Models.BurnRules.BurnRuleCreateRequest;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    [Permission(PermissionType.ActionRules, PermissionLevel.PartnerView)]
    public class BurnRulesController : ControllerBase
    {
        private readonly IExtRequestContext _requestContext;
        private readonly ICampaignClient _campaignsClient;
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly IImageService _imageService;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IVouchersClient _vouchersClient;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public BurnRulesController(
            IExtRequestContext requestContext,
            ICampaignClient campaignClient,
            ICurrencyConvertorClient currencyConverterClient,
            IImageService imageService,
            IPartnerManagementClient partnerManagementClient,
            IVouchersClient vouchersClient,
            ILogFactory logFactory,
            IMapper mapper)
        {
            _requestContext = requestContext;
            _campaignsClient = campaignClient;
            _currencyConverterClient = currencyConverterClient;
            _imageService = imageService;
            _partnerManagementClient = partnerManagementClient;
            _vouchersClient = vouchersClient;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Get all burn rules.
        /// </summary>
        /// <returns>
        /// A collection of burn rules.
        /// </returns>
        /// <response code="200">A collection of burn rules.</response>
        /// <response code="400">An error occurred while getting burn rules.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BurnRulesListResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<BurnRulesListResponse> GetBurnRulesListAsync([FromQuery] BurnRuleListRequest model)
        {
            var result = await _campaignsClient.BurnRules.GetAsync(
                _mapper.Map<BurnRulePaginationRequest>(model));

            #region put partner or global rate

            var burnRulesWithRate = new List<BurnRuleInfoResponse>();
            var burnRulesWithGlobalRate = new List<BurnRuleInfoResponse>();
            var partnersDict = new Dictionary<Guid, PartnerListDetailsModel>();
            // key is burnRuleId, value is partnerId
            var burnRulesWithPartnerDict = new Dictionary<Guid, Guid>();

            foreach (var rule in result.BurnRules)
            {
                if (rule.UsePartnerCurrencyRate)
                {
                    burnRulesWithRate.Add(rule);
                }
            }

            if (burnRulesWithRate.Count > 0)
            {
                var burnRulesWithRateDict = burnRulesWithRate.ToDictionary(x => x.Id);
                var burnRules = new List<BurnRuleResponse>();

                // load burn rules with PartnerIds
                foreach (var batch in burnRulesWithRate.Batch(10))
                {
                    var tasks = new List<Task<BurnRuleResponse>>();

                    foreach (var rule in batch)
                    {
                        tasks.Add(_campaignsClient.BurnRules.GetByIdAsync(rule.Id));
                    }

                    await Task.WhenAll(tasks);

                    burnRules.AddRange(tasks.Select(x => x.Result));
                }

                // check if there is one partner 
                // then put to array to load them (and put to burnRulesWithPartnerDict)
                // otherwise put to burnRulesWithGlobalRate
                foreach (var rule in burnRules)
                {
                    if (rule.PartnerIds.Length == 1)
                    {
                        partnersDict.TryAdd(rule.PartnerIds.First(), null);
                        burnRulesWithPartnerDict.Add(rule.Id, rule.PartnerIds.First());
                    }
                    else
                    {
                        burnRulesWithGlobalRate.Add(burnRulesWithRateDict[rule.Id]);
                    }
                }

                // load all partners
                if (partnersDict.Count > 0)
                {
                    var partnersList =
                        await _partnerManagementClient.Partners.GetByIdsAsync(partnersDict.Keys.ToArray());

                    foreach (var partner in partnersList)
                    {
                        if (partner != null)
                        {
                            partnersDict[partner.Id] = partner;
                        }
                    }
                }

                // final check in partner to use partner rate or global rate
                foreach (var ruleWithPartner in burnRulesWithPartnerDict)
                {
                    partnersDict.TryGetValue(ruleWithPartner.Value, out var partner);

                    if (partner != null && !partner.UseGlobalCurrencyRate)
                    {
                        burnRulesWithRateDict[ruleWithPartner.Key].AmountInCurrency = partner.AmountInCurrency;
                        burnRulesWithRateDict[ruleWithPartner.Key].AmountInTokens = partner.AmountInTokens;
                    }
                    else
                    {
                        burnRulesWithGlobalRate.Add(burnRulesWithRateDict[ruleWithPartner.Key]);
                    }
                }
            }

            // fill global rate
            if (burnRulesWithGlobalRate.Count > 0)
            {
                var globalCurrencyRate = await _currencyConverterClient.GlobalCurrencyRates.GetAsync();

                foreach (var rule in burnRulesWithGlobalRate)
                {
                    rule.AmountInCurrency = globalCurrencyRate.AmountInCurrency;
                    rule.AmountInTokens = globalCurrencyRate.AmountInTokens;
                }
            }

            #endregion

            return new BurnRulesListResponse
            {
                BurnRules = _mapper.Map<IEnumerable<BurnRuleInfoModel>>(result.BurnRules),
                PagedResponse = new PagedResponseModel
                {
                    CurrentPage = result.CurrentPage, TotalCount = result.TotalCount
                }
            };
        }

        /// <summary>
        /// Get burn rule by identifier.
        /// </summary>
        /// <param name="id">The identifier of the burn rule.</param>
        /// <returns>
        /// A burn rule.
        /// </returns>
        /// <response code="200">A burn rule.</response>
        /// <response code="400">An error occurred while getting the burn rule.</response>
        [HttpGet("query")]
        [ProducesResponseType(typeof(BurnRuleModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<BurnRuleModel> GetBurnRuleByIdAsync([FromQuery] string id)
        {
            if (!Guid.TryParse(id, out var idGuid))
            {
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidBurnRuleId);
            }

            var burnRuleResponse = await _campaignsClient.BurnRules.GetByIdAsync(idGuid);

            ThrowIfError(burnRuleResponse.ErrorCode, burnRuleResponse.ErrorMessage);

            var result = _mapper.Map<BurnRuleModel>(burnRuleResponse);

            if (burnRuleResponse.Vertical == Vertical.Retail)
            {
                var spendRuleVouchers = await _vouchersClient.Reports.GetSpendRuleVouchersAsync(Guid.Parse(id));

                result.VouchersCount = spendRuleVouchers.Total;
                result.VouchersInStockCount = spendRuleVouchers.InStock;
            }

            // dictionary by Localization
            var mobileContentsDictionary = new Dictionary<Localization, MobileContentResponse>();

            foreach (var content in burnRuleResponse.BurnRuleContents)
            {
                if (mobileContentsDictionary.TryGetValue(content.Localization, out var existingMobileContent))
                {
                    FillMobileContent(existingMobileContent);
                }
                else
                {
                    var newMobileContent = new MobileContentResponse {MobileLanguage = content.Localization};

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
        /// Creates new burn rule.
        /// </summary>
        /// <returns>
        /// The information about the created burn rule.
        /// </returns>
        /// <response code="200">Response model with BurnRuleId.</response>
        /// <response code="400">An error occurred while creating burn rule.</response>
        [HttpPost]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType(typeof(BurnRuleCreatedResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<BurnRuleCreatedResponse> CreateBurnRuleAsync([FromBody] BurnRuleCreateRequest model)
        {
            var request =
                _mapper.Map<BurnRuleCreateRequest, Lykke.Service.Campaign.Client.Models.BurnRule.Requests.BurnRuleCreateRequest>(
                    model,
                    opt => opt.AfterMap((src, dest) => { dest.CreatedBy = _requestContext.UserId; }));

            var mobileContents = new List<BurnRuleContentCreateRequest>();

            foreach (var mobileContent in model.MobileContents)
            {
                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new BurnRuleContentCreateRequest
                    {
                        Localization = mobileContent.MobileLanguage,
                        RuleContentType = RuleContentType.Title,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new BurnRuleContentCreateRequest
                {
                    Localization = mobileContent.MobileLanguage,
                    RuleContentType = RuleContentType.Description,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                // create content for adding image
                mobileContents.Add(new BurnRuleContentCreateRequest
                {
                    Localization = mobileContent.MobileLanguage,
                    RuleContentType = RuleContentType.UrlForPicture,
                    Value = null
                });
            }

            request.BurnRuleContents = mobileContents;

            BurnRuleCreateResponse response;

            try
            {
                response = await _campaignsClient.BurnRules.CreateAsync(request);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);

            var createdBurnRule = await _campaignsClient.BurnRules.GetByIdAsync(response.BurnRuleId);
            var createImageContents = new List<ImageContentCreatedResponse>();

            foreach (var content in createdBurnRule.BurnRuleContents)
            {
                if (content.RuleContentType == RuleContentType.UrlForPicture)
                {
                    createImageContents.Add(new ImageContentCreatedResponse
                    {
                        MobileLanguage = content.Localization, RuleContentId = content.Id
                    });
                }
            }

            return new BurnRuleCreatedResponse {Id = response.BurnRuleId, CreatedImageContents = createImageContents};
        }

        [HttpPost("vouchers")]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task AddVouchersAsync([FromQuery] [Required] Guid spendRuleId, [Required] IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0 || !formFile.FileName.EndsWith(".csv"))
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidFile);

            var codes = new List<string>();

            try
            {
                using (var reader = new StreamReader(formFile.OpenReadStream()))
                {
                    string line;

                    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                        codes.Add(line.Trim());
                }
            }
            catch (Exception exception)
            {
                _log.Error(exception, "An error occurred while parsing a CSV file that contains voucher codes",
                    $"spendRuleId: {spendRuleId}");

                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CanNotReadFile);
            }

            if (!codes.Any())
                return;

            VoucherCreateResultModel result;

            try
            {
                result = await _vouchersClient.Vouchers.AddAsync(new VoucherCreateModel
                {
                    Codes = codes, SpendRuleId = spendRuleId
                });
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            if (result.ErrorCode != VoucherErrorCode.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(result.ErrorCode.ToString()));
        }

        /// <summary>
        /// Updates burn rule.
        /// </summary>
        /// <response code="204">Burn rule successfully updated.</response>
        /// <response code="400">An error occurred while updating burn rule.</response>
        [HttpPut]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task UpdateBurnRuleAsync([FromBody] BurnRuleUpdateRequest model)
        {
            var request = _mapper.Map<BurnRuleEditRequest>(model);

            var mobileContents = new List<BurnRuleContentEditRequest>();

            foreach (var mobileContent in model.MobileContents)
            {
                if (!string.IsNullOrEmpty(mobileContent.Title))
                {
                    mobileContents.Add(new BurnRuleContentEditRequest
                    {
                        Id = mobileContent.TitleId,
                        RuleContentType = RuleContentType.Title,
                        Localization = mobileContent.MobileLanguage,
                        Value = mobileContent.Title
                    });
                }

                mobileContents.Add(new BurnRuleContentEditRequest
                {
                    Id = mobileContent.DescriptionId,
                    RuleContentType = RuleContentType.Description,
                    Localization = mobileContent.MobileLanguage,
                    Value = string.IsNullOrEmpty(mobileContent.Description) ? null : mobileContent.Description
                });

                mobileContents.Add(new BurnRuleContentEditRequest
                {
                    Id = mobileContent.ImageId,
                    RuleContentType = RuleContentType.UrlForPicture,
                    Localization = mobileContent.MobileLanguage,
                    Value = mobileContent.ImageBlobUrl
                });
            }

            request.BurnRuleContents = mobileContents;

            BurnRuleResponse response;

            try
            {
                response = await _campaignsClient.BurnRules.UpdateAsync(request);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Deletes burn rule.
        /// </summary>
        /// <param name="id">The burn rule identifier.</param>
        /// <response code="204">Burn rule successfully deleted.</response>
        /// <response code="400">An error occurred while deleting campaign.</response>
        [HttpDelete]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [NotSerializedFilter]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task DeleteBurnRuleAsync([FromQuery] string id)
        {
            if (!Guid.TryParse(id, out var idGuid))
            {
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidBurnRuleId);
            }

            var response = await _campaignsClient.BurnRules.DeleteAsync(idGuid);

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Adds an image to a burn rule.
        /// </summary>
        /// <param name="model">The image content.</param>
        /// <param name="formFile">The file</param>
        /// <response code="204">Burn rule image successfully added.</response>
        /// <response code="400">An error occurred while adding a burn rule image.</response>
        [HttpPost("image")]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task AddImageAsync([FromQuery] ImageAddRequest model, [Required] IFormFile formFile)
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
                response = await _campaignsClient.BurnRules.AddImage(requestModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Updates image of a burn rule.
        /// </summary>
        /// <param name="model">The image content.</param>
        /// <param name="formFile">The file.</param>
        /// <response code="204">Image successfully updated.</response>
        /// <response code="400">An error occurred while updating an image.</response>
        [HttpPut("image")]
        [Permission(PermissionType.ActionRules, PermissionLevel.Edit)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task UpdateImageAsync([FromQuery] ImageEditRequest model, [Required] IFormFile formFile)
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
                response = await _campaignsClient.BurnRules.UpdateImage(requestModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        private static void ThrowIfError(CampaignServiceErrorCodes errorCode, string message)
        {
            if (errorCode != CampaignServiceErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(errorCode.ToString(), message));
        }
    }
}
