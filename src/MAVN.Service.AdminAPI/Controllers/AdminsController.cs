using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.AdminManagement.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Admins;
using MAVN.Service.AdminAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;
using SuggestedValueMapping = MAVN.Service.AdminAPI.Models.Admins.SuggestedValueMapping;
using SuggestedValueType = MAVN.Service.AdminAPI.Models.Admins.SuggestedValueType;
using Microsoft.AspNetCore.Authorization;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.StringUtils;
using MAVN.Service.AuditLogs.Contract.Events;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ICredentialsGeneratorService _credentialsGeneratorService;
        private readonly IExtRequestContext _requestContext;
        private readonly IAdminsService _adminsService;
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;
        private readonly IAuditLogPublisher _auditLogPublisher;
        private readonly IMapper _mapper;

        public AdminsController(
            IAdminsService adminsService,
            ICredentialsGeneratorService credentialsGeneratorService,
            IExtRequestContext requestContext,
            IMapper mapper,
            IAdminManagementServiceClient adminManagementServiceClient,
            IAuditLogPublisher auditLogPublisher)
        {
            _adminsService = adminsService;
            _mapper = mapper;
            _adminManagementServiceClient = adminManagementServiceClient;
            _auditLogPublisher = auditLogPublisher;
            _credentialsGeneratorService = credentialsGeneratorService;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Registers partner's admin.
        /// </summary>
        /// <param name="model">Admin data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while creating admin.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AdminModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<AdminModel> RegisterPartnerAdminAsync([FromBody] AdminRegisterModel model)
        {
            try
            {
                var (error, admin) = await _adminsService.RegisterPartnerAdminAsync(model);

                if (error == AdminServiceCreateResponseError.None)
                {
                    model.Password = null;
                    model.Email = model.Email.SanitizeEmail();
                    model.CompanyName = model.CompanyName.SanitizeName();
                    await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.PartnerAdminCreate);
                    return _mapper.Map<AdminModel>(admin);
                }

                switch (error)
                {
                    case AdminServiceCreateResponseError.AlreadyRegistered:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminAlreadyRegistered);
                    case AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailOrPasswordFormat);
                    default:
                        throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(error.ToString()));
                }
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }
        }

        /// <summary>
        /// Registers admin.
        /// </summary>
        /// <param name="model">Admin data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while creating admins.</response>
        [HttpPost]
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AdminModel> CreateAdminAsync([FromBody] AdminCreateModel model)
        {
            var (error, admin) = await _adminsService.RegisterAsync(
                model.Email,
                model.Password,
                model.PhoneNumber,
                model.FirstName,
                model.LastName,
                model.Company,
                model.Department,
                model.JobTitle);

            if (error == AdminServiceCreateResponseError.None)
            {
                model.Password = null;
                model.Email = model.Email.SanitizeEmail();
                model.FirstName = model.FirstName.SanitizeName();
                model.LastName = model.LastName.SanitizeName();
                model.PhoneNumber = model.PhoneNumber.SanitizePhone();
                model.Company = model.Company.SanitizeName();
                model.Department = model.Department.SanitizeName();
                await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.AdminCreate);
                return _mapper.Map<AdminModel>(admin);
            }

            switch (error)
            {
                case AdminServiceCreateResponseError.AlreadyRegistered:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminAlreadyRegistered);
                case AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailOrPasswordFormat);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates admin.
        /// </summary>
        /// <param name="model">Admin data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while updating admins.</response>
        [HttpPut]
        [Permission(
            PermissionType.AdminUsers,
            new[]
            {
                PermissionLevel.Edit,
                PermissionLevel.PartnerEdit,
            }
        )]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AdminModel> UpdateAdminAsync([FromBody] AdminUpdateModel model)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.AdminUsers);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // filter data for current _requestContext.UserId
                if (model.Id != Guid.Parse(_requestContext.UserId))
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var (error, admin) = await _adminsService.UpdateAdminAsync(
                model.Id.ToString(),
                model.PhoneNumber,
                model.FirstName,
                model.LastName,
                model.Company,
                model.Department,
                model.JobTitle,
                model.IsActive);

            if (error == AdminServiceResponseError.None)
            {
                model.FirstName = model.FirstName.SanitizeName();
                model.LastName = model.LastName.SanitizeName();
                model.PhoneNumber = model.PhoneNumber.SanitizePhone();
                model.Company = model.Company.SanitizeName();
                model.Department = model.Department.SanitizeName();
                await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.AdminUpdate);
                return _mapper.Map<AdminModel>(admin);
            }

            switch (error)
            {
                case AdminServiceResponseError.AdminUserDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates admin permissions.
        /// </summary>
        /// <param name="model">Permissions data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while updating admins.</response>
        [HttpPut("permissions")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AdminModel> UpdatePermissionsAsync([FromBody] AdminUpdatePermissionsModel model)
        {
            var (error, admin) = await _adminsService.UpdateAdminPermissionsAsync(
                model.AdminUserId.ToString(),
                _mapper.Map<List<Permission>>(model.Permissions));

            if (error == AdminServiceResponseError.None)
            {
                await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.UpdateAdminPermissions);
                return _mapper.Map<AdminModel>(admin);
            }

            switch (error)
            {
                case AdminServiceResponseError.AdminUserDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns an auto-generated password to be used as a suggestion to the admin.
        /// </summary>
        /// <returns>An auto generated password.</returns>
        [ProducesResponseType(typeof(GeneratedPasswordModel), (int)HttpStatusCode.OK)]
        [HttpGet("generateSuggestedPassword")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.View)]
        [LykkeAuthorizeWithoutCache]
        public async Task<GeneratedPasswordModel> GetSuggestedPasswordAsync()
        {
            return new GeneratedPasswordModel
            {
                Password = await _credentialsGeneratorService.GenerateRandomPasswordForAdminAsync()
            };
        }
        
        /// <summary>
        /// Returns a list of values to be suggested to a client as autofill data.
        /// </summary>
        /// <returns>A list of values.</returns>
        [ProducesResponseType(typeof(List<SuggestedValueMapping>), (int)HttpStatusCode.OK)]
        [HttpGet("autofillData")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.View)]
        [LykkeAuthorizeWithoutCache]
        public async Task<List<SuggestedValueMapping>> GetAutofillDataAsync()
        {
            var values = await _adminManagementServiceClient.AdminsApi.GetAutofillValuesAsync();

            return values.Values.Select(x => new SuggestedValueMapping
                {
                    Type = _mapper.Map<SuggestedValueType>(x.Type),
                    Values = x.Values
                })
                .ToList();
        }

        /// <summary>
        /// Returns specific page of admin list.
        /// </summary>
        /// <param name="model">The pagination request parameters.</param>
        /// <returns>
        /// A collection of admins.
        /// </returns>
        /// <response code="200">A collection of admins.</response>
        /// <response code="400">An error occurred while getting admins.</response>
        [HttpPost("search")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.View)]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<AdminListResponse> GetAdminAsync([FromBody] AdminListRequest model)
        {
            var (pageSize, pageNumber, admins) = await _adminsService.GetAsync(model.PageSize, model.CurrentPage, model.SearchValue, model.Active);
            
            return new AdminListResponse
            {
                Items = _mapper.Map<List<AdminModel>>(admins),
                PagedResponse = new PagedResponseModel(pageNumber, pageSize)
            };
        }

        /// <summary>
        /// Get admin user by identifier.
        /// </summary>
        /// <param name="adminUserId">The identifier of admin user.</param>
        /// <returns>
        /// An admin user.
        /// </returns>
        /// <response code="200">An admin user.</response>
        /// <response code="400">An error occurred while getting admin.</response>
        [HttpGet("query")]
        [Permission(
            PermissionType.AdminUsers,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<AdminModel> GetAdminByIdAsync([FromQuery][Required] Guid adminUserId)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.AdminUsers);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // filter data for current _requestContext.UserId
                if (adminUserId != Guid.Parse(_requestContext.UserId))
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var (error, admin) = await _adminsService.GetAsync(adminUserId.ToString());

            switch (error)
            {
                case AdminServiceResponseError.None:
                    return _mapper.Map<AdminModel>(admin);
                case AdminServiceResponseError.AdminUserDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns a list of all possible permission types.
        /// </summary>
        /// <returns>
        /// A list of all possible permission types.
        /// </returns>
        /// <response code="200">A collection of all permission types.</response>
        [HttpGet("permissions")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.View)]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminListResponse), (int)HttpStatusCode.OK)]
        public AdminPermissionsTypesResponse GetAllPermissionTypes()
        {
            var permissions = _adminsService.GetAllPermissions();

            return new AdminPermissionsTypesResponse
            {
                Types = permissions
            };
        }
        
        /// <summary>
        /// Reset admin password.
        /// </summary>
        /// <param name="model">The identifier of admin user.</param>
        /// <returns>
        /// An admin user.
        /// </returns>
        /// <response code="200">An admin user.</response>
        /// <response code="400">An error occurred while getting admin.</response>
        [HttpPost("resetPassword")]
        [Permission(
            PermissionType.AdminUsers,
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType(typeof(AdminModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<AdminModel> ResetPasswordAsync([FromBody]AdminResetPasswordModel model)
        {
            #region Filter

            var permissionLevel = await _requestContext.GetPermissionLevelAsync(PermissionType.AdminUsers);

            if (permissionLevel.HasValue && permissionLevel.Value == PermissionLevel.PartnerEdit)
            {
                // filter data for current _requestContext.UserId
                if (model.AdminId != _requestContext.UserId)
                    throw LykkeApiErrorException.Forbidden(new LykkeApiErrorCode(nameof(HttpStatusCode.Forbidden)));
            }

            #endregion

            var (error, admin) = await _adminsService.ResetPasswordAsync(model.AdminId);

            if (error == AdminResetPasswordErrorCodes.None)
            {
                await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.ResetAdminPassword);
                return _mapper.Map<AdminModel>(admin);
            }
            switch (error)
            {
                case AdminResetPasswordErrorCodes.AdminUserDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
