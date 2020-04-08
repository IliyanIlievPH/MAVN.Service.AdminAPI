using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.AdminManagement.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Admins;
using MAVN.Service.AdminAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;
using SuggestedValueMapping = MAVN.Service.AdminAPI.Models.Admins.SuggestedValueMapping;
using SuggestedValueType = MAVN.Service.AdminAPI.Models.Admins.SuggestedValueType;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.AdminUsers, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ICredentialsGeneratorService _credentialsGeneratorService;
        private readonly IAdminsService _adminsService;
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;
        private readonly IMapper _mapper;

        public AdminsController(
            IAdminsService adminsService,
            ICredentialsGeneratorService credentialsGeneratorService,
            IMapper mapper,
            IAdminManagementServiceClient adminManagementServiceClient)
        {
            _adminsService = adminsService;
            _mapper = mapper;
            _adminManagementServiceClient = adminManagementServiceClient;
            _credentialsGeneratorService = credentialsGeneratorService;
        }

        /// <summary>
        /// Registers admin.
        /// </summary>
        /// <param name="model">Admin data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while creating admins.</response>
        [HttpPost]
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
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

            switch (error)
            {
                case AdminServiceCreateResponseError.None:
                    return _mapper.Map<AdminModel>(admin);
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
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
        [ProducesResponseType(typeof(AdminModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AdminModel> UpdateAdminAsync([FromBody] AdminUpdateModel model)
        {
            var (error, admin) = await _adminsService.UpdateAdminAsync(
                model.Id.ToString(),
                model.PhoneNumber,
                model.FirstName,
                model.LastName,
                model.Company,
                model.Department,
                model.JobTitle,
                model.IsActive);

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
        /// Updates admin permissions.
        /// </summary>
        /// <param name="model">Permissions data.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="400">An error occurred while updating admins.</response>
        [HttpPut("permissions")]
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
        [ProducesResponseType(typeof(AdminModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AdminModel> UpdatePermissionsAsync([FromBody] AdminUpdatePermissionsModel model)
        {
            var (error, admin) = await _adminsService.UpdateAdminPermissionsAsync(
                model.AdminUserId.ToString(),
                _mapper.Map<List<Permission>>(model.Permissions));

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
        /// Returns an auto-generated password to be used as a suggestion to the admin.
        /// </summary>
        /// <returns>An auto generated password.</returns>
        [ProducesResponseType(typeof(GeneratedPasswordModel), (int)HttpStatusCode.OK)]
        [HttpGet("generateSuggestedPassword")]
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
        [ProducesResponseType(typeof(AdminModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<AdminModel> GetAdminByIdAsync([FromQuery][Required] Guid adminUserId)
        {
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
        [ProducesResponseType(typeof(AdminListResponse), (int)HttpStatusCode.OK)]
        public AdminPermissionsTypesResponse GetAllPermissionTypes()
        {
            var permissions = _adminsService.GetAllPermissions();

            return new AdminPermissionsTypesResponse
            {
                Types = _mapper.Map<List<AdminPermissionType>>(permissions)
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
        [ProducesResponseType(typeof(AdminModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Permission(PermissionType.AdminUsers, PermissionLevel.Edit)]
        public async Task<AdminModel> ResetPasswordAsync([FromBody]AdminResetPasswordModel model)
        {
            var (error, admin) = await _adminsService.ResetPasswordAsync(model.AdminId);

            switch (error)
            {
                case AdminResetPasswordErrorCodes.None:
                    return _mapper.Map<AdminModel>(admin);
                case AdminResetPasswordErrorCodes.AdminUserDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
