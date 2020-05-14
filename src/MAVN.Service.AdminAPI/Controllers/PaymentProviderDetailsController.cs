using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.PaymentProviderDetails;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.PaymentManagement.Client.Models.Requests;
using MAVN.Service.PaymentManagement.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using AvailablePaymentProvidersRequirementsResponse = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.AvailablePaymentProvidersRequirementsResponse;
using CreatePaymentProviderDetailsRequest = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.CreatePaymentProviderDetailsRequest;
using EditPaymentProviderDetailsRequest = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.EditPaymentProviderDetailsRequest;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using System.Linq;
using MAVN.Service.AdminAPI.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(
        PermissionType.ProgramPartners,
        new[]
        {
            PermissionLevel.View,
            PermissionLevel.PartnerEdit,
        }
    )]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class PaymentProviderDetailsController : ControllerBase
    {
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IPaymentManagementClient _paymentManagementClient;
        private readonly IExtRequestContext _requestContext;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IMapper _mapper;

        public PaymentProviderDetailsController(
            IPartnerManagementClient partnerManagementClient,
            IPaymentManagementClient paymentManagementClient,
            IExtRequestContext requestContext,
            ICustomerProfileClient customerProfileClient,
            IMapper mapper)
        {
            _partnerManagementClient = partnerManagementClient;
            _paymentManagementClient = paymentManagementClient;
            _requestContext = requestContext;
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
            await VerifyPermissionForPartnerAdmin(partnerId);

            var details = await _customerProfileClient.PaymentProviderDetails.GetListByPartnerIdAsync(partnerId);

            await FilterSecretData(details);

            var result = new PaymentProviderDetailsResponse
            {
                PaymentProviderDetails = _mapper.Map<IReadOnlyList<PaymentProviderDetails>>(details)
            };

            return result;
        }

        /// <summary>
        /// Filter secret data for SuperAdmin
        /// </summary>
        private async Task FilterSecretData(IReadOnlyList<CustomerProfile.Client.Models.Responses.PaymentProviderDetails> details)
        {
            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && (permissionLevel.Value == PermissionLevel.View || permissionLevel.Value == PermissionLevel.Edit))
            {
                var paymentProvidersRequirements = await _paymentManagementClient.Api.GetAvailablePaymentProvidersRequirementsAsync();

                foreach (var detail in details)
                {
                    var currentProviderRequirements = paymentProvidersRequirements.ProvidersRequirements.FirstOrDefault(_ => _.PaymentProvider == detail.PaymentIntegrationProvider);

                    if (currentProviderRequirements == null)
                        continue;

                    var secretProperties = currentProviderRequirements.Properties.Where(_ => _.IsSecret).Select(_ => _.JsonKey);

                    if (secretProperties.Count() > 0)
                    {
                        var jobj = (JObject)JsonConvert.DeserializeObject(detail.PaymentIntegrationProperties);

                        foreach (var secretProperty in secretProperties)
                        {
                            var secretPropertyValue = jobj[secretProperty]?.ToString();

                            if (!string.IsNullOrWhiteSpace(secretPropertyValue))
                            {
                                jobj[secretProperty] = new string('*', secretPropertyValue.Length);
                            }
                        }

                        detail.PaymentIntegrationProperties = JsonConvert.SerializeObject(jobj);
                    }
                }
            }
        }

        /// <summary>
        /// Create payment provider details
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task CreatePaymentProviderDetailsAsync([FromBody] CreatePaymentProviderDetailsRequest request)
        {
            await VerifyPermissionForPartnerAdmin(request.PartnerId);

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
            await VerifyPermissionForPartnerAdmin(request.PartnerId);

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

        /* TODO: implement when will be 2 or more payment providers
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
        */

        private async Task VerifyPermissionForPartnerAdmin(Guid partnerId)
        {
            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.ProgramPartners);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                var partners = await _partnerManagementClient.Partners.GetAsync(new PartnerListRequestModel
                {
                    CreatedBy = Guid.Parse(_requestContext.UserId),
                    CurrentPage = 1,
                    PageSize = Constants.MaxPageSize
                }); ;

                if (!partners.PartnersDetails.Any(_ => _.Id.Equals(partnerId)))
                {
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
                }
            }
        }
    }
}
