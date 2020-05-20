using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.AdminAPI.Models.Kyc.Requests;
using MAVN.Service.AdminAPI.Models.Kyc.Responses;
using MAVN.Service.Kyc.Client;
using MAVN.Service.Kyc.Client.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using KycInformationResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycInformationResponse;
using KycStatusChangeResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycStatusChangeResponse;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KycController : ControllerBase
    {
        private readonly IKycClient _kycClient;
        private readonly IMapper _mapper;

        public KycController(IKycClient kycClient, IMapper mapper)
        {
            _kycClient = kycClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get current kyc info
        /// </summary>
        /// <param name="partnerId"></param>
        [HttpGet("current")]
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
        [ProducesResponseType(typeof(KycStatusChangeResponse), (int)HttpStatusCode.OK)]
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
        [ProducesResponseType(typeof(KycInformationUpdateResponse), (int)HttpStatusCode.OK)]
        public async Task<KycInformationUpdateResponse> UpdateKycInfoAsync([FromBody]KycInformationUpdateRequest request)
        {
            var model = _mapper.Map<KycUpdateRequest>(request);
            var result = await _kycClient.KycApi.UpdateKycInfoAsync(model);

            return _mapper.Map<KycInformationUpdateResponse>(result);
        }
    }
}
