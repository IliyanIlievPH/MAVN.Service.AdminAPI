using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Kyc.Requests;
using MAVN.Service.AdminAPI.Models.Kyc.Responses;
using MAVN.Service.Kyc.Client;
using MAVN.Service.Kyc.Client.Models.Enums;
using MAVN.Service.Kyc.Client.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using KycInformationResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycInformationResponse;
using KycStatusChangeResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycStatusChangeResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("api/[controller]")]
    public class KycController : ControllerBase
    {
        private readonly IKycClient _kycClient;
        private readonly IAuditLogPublisher _auditLogPublisher;
        private readonly IExtRequestContext _requestContext;
        private readonly IMapper _mapper;

        public KycController(
            IKycClient kycClient,
            IAuditLogPublisher auditLogPublisher,
            IMapper mapper,
            IExtRequestContext requestContext)
        {
            _kycClient = kycClient;
            _auditLogPublisher = auditLogPublisher;
            _mapper = mapper;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Get current kyc info
        /// </summary>
        /// <param name="partnerId"></param>
        [HttpGet("current")]
        [Permission(
            PermissionType.ProgramPartners,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(KycInformationResponse), (int)HttpStatusCode.OK)]
        public async Task<KycInformationResponse> GetCurrentByPartnerIdAsync([FromQuery]Guid partnerId)
        {
            var result = await _kycClient.KycApi.GetCurrentByPartnerIdAsync(partnerId);

            return _mapper.Map<KycInformationResponse>(result);
        }

        /// <summary>
        /// Get history of kyc info
        /// </summary>
        /// <param name="partnerId"></param>
        [HttpGet("history")]
        [Permission(
            PermissionType.ProgramPartners,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(IReadOnlyList<KycStatusChangeResponse>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<KycStatusChangeResponse>> GetKycStatusChangeHistoryByPartnerIdAsync([FromQuery]Guid partnerId)
        {
            var result = await _kycClient.KycApi.GetKycStatusChangeHistoryByPartnerIdAsync(partnerId);

            return _mapper.Map<IReadOnlyList<KycStatusChangeResponse>>(result);
        }

        /// <summary>
        /// update  kyc info
        /// </summary>
        /// <param name="request"></param>
        [HttpPut]
        [Permission(PermissionType.ProgramPartners, PermissionLevel.Edit)]
        [ProducesResponseType(typeof(KycInformationUpdateResponse), (int)HttpStatusCode.OK)]
        public async Task<KycInformationUpdateResponse> UpdateKycInfoAsync([FromBody]KycInformationUpdateRequest request)
        {
            var adminUserId = Guid.Parse(_requestContext.UserId);
            var model = new KycUpdateRequest()
            {
                PartnerId = request.PartnerId,
                Comment = request.Comment,
                KycStatus = (KycStatus)request.KycStatus,
                AdminUserId = adminUserId,
            };
            var result = await _kycClient.KycApi.UpdateKycInfoAsync(model);

            await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, request.ToJson(), ActionType.ChangeKycStatus);
            return _mapper.Map<KycInformationUpdateResponse>(result);
        }
    }
}
