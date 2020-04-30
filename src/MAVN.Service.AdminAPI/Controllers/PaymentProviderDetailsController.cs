using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.PaymentProviderDetails;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.PaymentManagement.Client.Models.Requests;
using MAVN.Service.PaymentManagement.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using AvailablePaymentProvidersRequirementsResponse = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.AvailablePaymentProvidersRequirementsResponse;
using CreatePaymentProviderDetailsRequest = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.CreatePaymentProviderDetailsRequest;
using EditPaymentProviderDetailsRequest = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.EditPaymentProviderDetailsRequest;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.ActionRules, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class PaymentProviderDetailsController : ControllerBase
    {
        private readonly IPaymentManagementClient _paymentManagementClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IMapper _mapper;

        public PaymentProviderDetailsController(
            IPaymentManagementClient paymentManagementClient,
            ICustomerProfileClient customerProfileClient,
            IMapper mapper)
        {
            _paymentManagementClient = paymentManagementClient;
            _customerProfileClient = customerProfileClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the required properties for the available payment providers
        /// </summary>
        [HttpGet("properties")]
        [ProducesResponseType(typeof(AvailablePaymentProvidersRequirementsResponse), (int)HttpStatusCode.OK)]
        public async Task<AvailablePaymentProvidersRequirementsResponse> GetAvailablePaymentProvidersRequirementsAsync()
        {
            var paymentProvidersRequirements = await _paymentManagementClient.Api.GetAvailablePaymentProvidersRequirementsAsync();

            var result = _mapper.Map<AvailablePaymentProvidersRequirementsResponse>(paymentProvidersRequirements);

            return result;
        }

        /// <summary>
        /// Get the required properties for the available payment providers
        /// </summary>
        [HttpGet("integration/check")]
        [ProducesResponseType(typeof(CheckPaymentIntegrationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CheckPaymentIntegrationResponse> CheckPaymentIntegrationAsync([FromQuery] CheckPaymentIntegrationRequest request)
        {
            var checkResult = await _paymentManagementClient.Api.CheckPaymentIntegrationAsync(new PaymentIntegrationCheckRequest
            {
                PartnerId = request.PartnerId
            });

            switch (checkResult)
            {
                case CheckPaymentIntegrationErrorCode.None:
                    return new CheckPaymentIntegrationResponse { IsConfiguredCorrectly = true };
                case CheckPaymentIntegrationErrorCode.Fail:
                case CheckPaymentIntegrationErrorCode.PartnerConfigurationNotFound:
                case CheckPaymentIntegrationErrorCode.PartnerConfigurationPropertyIsMissing:
                    return new CheckPaymentIntegrationResponse { IsConfiguredCorrectly = false, Error = checkResult.ToString() };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Get list of payment provider details by partner id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaymentProviderDetailsResponse), (int)HttpStatusCode.OK)]
        public async Task<PaymentProviderDetailsResponse> GetPaymentProviderDetailsByPartnerIdAsync([FromQuery] Guid partnerId)
        {
            var details = await _customerProfileClient.PaymentProviderDetails.GetListByPartnerIdAsync(partnerId);

            var result = new PaymentProviderDetailsResponse
            {
                PaymentProviderDetails = _mapper.Map<IReadOnlyList<PaymentProviderDetails>>(details)
            };

            return result;
        }

        /// <summary>
        /// Create payment provider details
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task CreatePaymentProviderDetailsAsync([FromBody] CreatePaymentProviderDetailsRequest request)
        {
            var error = await _customerProfileClient.PaymentProviderDetails.CreateAsync(new CustomerProfile.Client.Models.Requests.CreatePaymentProviderDetailsRequest
            {
                PartnerId = request.PartnerId,
                PaymentIntegrationProperties = request.PaymentIntegrationProperties,
                PaymentIntegrationProvider = request.PaymentIntegrationProvider,
            });

            if (error != PaymentProviderDetailsErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
        }

        /// <summary>
        /// Update payment provider details
        /// </summary>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task EditPaymentProviderDetailsAsync([FromBody] EditPaymentProviderDetailsRequest request)
        {
            var error = await _customerProfileClient.PaymentProviderDetails.UpdateAsync(new CustomerProfile.Client.Models.Requests.EditPaymentProviderDetailsRequest
            {
                PartnerId = request.PartnerId,
                PaymentIntegrationProperties = request.PaymentIntegrationProperties,
                PaymentIntegrationProvider = request.PaymentIntegrationProvider,
                Id = request.Id,
            });

            if (error != PaymentProviderDetailsErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
        }

        /// <summary>
        /// Update payment provider details
        /// </summary>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task DeletePaymentProviderDetailsAsync([FromQuery] Guid id)
        {
            var error = await _customerProfileClient.PaymentProviderDetails.DeleteAsync(id);

            if (error != PaymentProviderDetailsErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
        }
    }
}
