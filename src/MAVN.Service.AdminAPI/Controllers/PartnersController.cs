using System;
using System.Collections.Generic;
using System.Linq;
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
using MAVN.Service.AdminAPI.Models.Kyc.Enum;
using MAVN.Service.AdminAPI.Models.Partners.Requests;
using MAVN.Service.AdminAPI.Models.Partners.Responses;
using MAVN.Service.Kyc.Client;
using MAVN.Service.Kyc.Client.Models.Responses;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Enums;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using Microsoft.AspNetCore.Mvc;
using PartnerCreateResponse = MAVN.Service.AdminAPI.Models.Partners.Responses.PartnerCreateResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(
        new[] {
            PermissionType.ProgramPartners,
            PermissionType.ActionRules
        },
        new[]
        {
            PermissionLevel.View,
            PermissionLevel.PartnerEdit,
        }
    )]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class PartnersController : ControllerBase
    {
        private readonly IExtRequestContext _requestContext;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IKycClient _kycClient;
        private readonly IMapper _mapper;

        public PartnersController(
            IExtRequestContext requestContext,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper,
            IKycClient kycClient)
        {
            _requestContext = requestContext ??
                              throw new ArgumentNullException(nameof(requestContext));
            _partnerManagementClient = partnerManagementClient ??
                                       throw new ArgumentNullException(nameof(partnerManagementClient));
            _mapper = mapper ??
                      throw new ArgumentNullException(nameof(mapper));
            _kycClient = kycClient;
        }

        /// <summary>
        /// Get all partners.
        /// </summary>
        /// <returns>
        /// A collection of partners.
        /// </returns>
        /// <response code="200">A collection of partners.</response>
        /// <response code="400">An error occurred while getting partners.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<PartnersListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PartnersListResponse> GetAllPartnersAsync([FromQuery] PartnerListRequest request)
        {
            var requestModel = _mapper.Map<PartnerListRequestModel>(request);

            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                requestModel.CreatedBy = Guid.Parse(_requestContext.UserId);
            }

            #endregion

            var result =
                await _partnerManagementClient.Partners.GetAsync(requestModel);

            var response = new PartnersListResponse
            {
                PagedResponse =
                    new PagedResponseModel
                    {
                        TotalCount = result.TotalSize,
                        CurrentPage = result.CurrentPage
                    },
                Partners = _mapper.Map<IEnumerable<PartnerRowResponse>>(result.PartnersDetails)
            };

            await PopulateResponseWithPartnerKycStatus(response);

            return response;
        }

        /// <summary>
        /// Check if partner has ability to do something
        /// </summary>
        /// <param name="request">.</param>
        /// <response code="200">Check ability response.</response>
        [HttpGet("ability/check")]
        [ProducesResponseType(typeof(CheckPartnerAbilityResponse), (int)HttpStatusCode.OK)]
        public async Task<CheckPartnerAbilityResponse> CheckPartnerAbilityAsync([FromQuery] CheckPartnerAbilityRequest request)
        {
            var requestModel = _mapper.Map<CheckAbilityRequest>(request);
            var result = await _partnerManagementClient.Partners.CheckAbilityAsync(requestModel);

            return _mapper.Map<CheckPartnerAbilityResponse>(result);
        }

        /// <summary>
        /// Get partner by identifier.
        /// </summary>
        /// <param name="id">The partner identifier.</param>
        /// <returns>
        /// A campaign.
        /// </returns>
        /// <response code="200">A partner.</response>
        /// <response code="400">An error occurred while getting partner.</response>
        [HttpGet("query")]
        [ProducesResponseType(typeof(PartnerDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PartnerDetailsResponse> GetPartnerByIdAsync([FromQuery] Guid id)
        {
            var response = await _partnerManagementClient.Partners.GetByIdAsync(id);

            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // filter data for current _requestContext.UserId
                if (response.CreatedBy != Guid.Parse(_requestContext.UserId))
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            return _mapper.Map<PartnerDetailsResponse>(response);
        }

        /// <summary>
        /// Adds new partner.
        /// </summary>
        /// <response code="200">The partner successfully added..</response>
        /// <response code="400">An error occurred while adding partner.</response>
        [HttpPost]
        [Permission(
            PermissionType.ProgramPartners,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(PartnerCreateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PartnerCreateResponse> AddAsync([FromBody] PartnerCreateRequest request)
        {
            var requestMapped = _mapper.Map<PartnerCreateRequest, PartnerCreateModel>(request,
                opt => opt.AfterMap((src, dest) => { dest.CreatedBy = Guid.Parse(_requestContext.UserId); }));

            requestMapped.ClientId = await _partnerManagementClient.Auth.GenerateClientId();
            requestMapped.ClientSecret = await _partnerManagementClient.Auth.GenerateClientSecret();

            PartnerManagement.Client.Models.Partner.PartnerCreateResponse response;

            try
            {
                response = await _partnerManagementClient.Partners.CreateAsync(requestMapped);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);

            return new PartnerCreateResponse
            {
                PartnerId = response.Id.ToString()
            };
        }

        /// <summary>
        /// Updates partner.
        /// </summary>
        /// <response code="200">Partner successfully update.</response>
        /// <response code="400">An error occurred while updating partner.</response>
        [HttpPut]
        [Permission(
            PermissionType.ProgramPartners,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UpdatePartnerAsync([FromBody] PartnerUpdateRequest request)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                var existingPartner = await _partnerManagementClient.Partners.GetByIdAsync(request.Id);

                // filter data for current _requestContext.UserId
                if (existingPartner != null &&
                    existingPartner.CreatedBy != Guid.Parse(_requestContext.UserId))
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var requestModel = _mapper.Map<PartnerUpdateModel>(request);

            PartnerUpdateResponse response;

            try
            {
                response = await _partnerManagementClient.Partners.UpdateAsync(requestModel);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
        }

        /// <summary>
        /// Generated new client secret
        /// </summary>
        /// <response code="200">The partner successfully generated client secret.</response>
        /// <response code="400">An error occurred while generating client secret.</response>
        [HttpPost("generateClientSecret")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<string> GenerateClientSecretAsync()
        {
            return await _partnerManagementClient.Auth.GenerateClientSecret();
        }

        /// <summary>
        /// Generated new client id
        /// </summary>
        /// <response code="200">The partner successfully generated client id.</response>
        /// <response code="400">An error occurred while generating client id.</response>
        [HttpPost("generateClientId")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<string> GenerateClientIdAsync()
        {
            return await _partnerManagementClient.Auth.GenerateClientId();
        }

        private static void ThrowIfError(PartnerManagementError errorCode, string message)
        {
            if (errorCode != PartnerManagementError.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(errorCode.ToString(), message));
        }

        private async Task PopulateResponseWithPartnerKycStatus(PartnersListResponse response)
        {
            var partnerIds = response.Partners.Select(e => e.Id).ToArray();
            var kycInformationResponses = await _kycClient.KycApi.GetCurrentByPartnerIdsAsync(partnerIds);

            SetPartnerKycStatus(response, kycInformationResponses);
        }

        private void SetPartnerKycStatus(PartnersListResponse response, IReadOnlyList<KycInformationResponse> kycInformationResponses)
        {
            var dict = response.Partners.ToDictionary(k => k.Id, v => v);
            foreach (var kycInformation in kycInformationResponses)
            {
                dict.TryGetValue(kycInformation.PartnerId, out var partnerRowResponse);
                if (partnerRowResponse != null)
                {
                    partnerRowResponse.KycStatus = (KycStatus)kycInformation.KycStatus;
                }
            }

        }
    }
}
