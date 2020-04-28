using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Enums;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.Partners.Requests;
using MAVN.Service.AdminAPI.Models.Partners.Responses;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IMapper _mapper;

        public PartnersController(
            IExtRequestContext requestContext,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper)
        {
            _requestContext = requestContext ??
                              throw new ArgumentNullException(nameof(requestContext));
            _partnerManagementClient = partnerManagementClient ??
                                       throw new ArgumentNullException(nameof(partnerManagementClient));
            _mapper = mapper ??
                      throw new ArgumentNullException(nameof(mapper));
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
        [ProducesResponseType(typeof(IReadOnlyList<PartnersListResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<PartnersListResponse> GetAllPartnersAsync([FromQuery] PartnerListRequest request)
        {
            var requestModel = _mapper.Map<PartnerListRequestModel>(request);

            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // TODO: filter data for current _requestContext.UserId
            }

            #endregion

            var result =
                await _partnerManagementClient.Partners.GetAsync(requestModel);

            return new PartnersListResponse
            {
                PagedResponse =
                    new PagedResponseModel {TotalCount = result.TotalSize, CurrentPage = result.CurrentPage},
                Partners = _mapper.Map<IEnumerable<PartnerRowResponse>>(result.PartnersDetails)
            };
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
        /// <response code="204">The partner successfully added..</response>
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task AddAsync([FromBody] PartnerCreateRequest request)
        {
            var requestMapped = _mapper.Map<PartnerCreateRequest, PartnerCreateModel>(request,
                opt => opt.AfterMap((src, dest) => { dest.CreatedBy = Guid.Parse(_requestContext.UserId); }));

            requestMapped.ClientId = await _partnerManagementClient.Auth.GenerateClientId();
            requestMapped.ClientSecret = await _partnerManagementClient.Auth.GenerateClientSecret();
            
            PartnerCreateResponse response;

            try
            {
                response = await _partnerManagementClient.Partners.CreateAsync(requestMapped);
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }

            ThrowIfError(response.ErrorCode, response.ErrorMessage);
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<string> GenerateClientIdAsync()
        {
            return await _partnerManagementClient.Auth.GenerateClientId();
        }

        private static void ThrowIfError(PartnerManagementError errorCode, string message)
        {
            if (errorCode != PartnerManagementError.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(errorCode.ToString(), message));
        }
    }
}
