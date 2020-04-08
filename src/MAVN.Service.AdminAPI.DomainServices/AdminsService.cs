using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Lykke.Service.AdminManagement.Client;
using Lykke.Service.AdminManagement.Client.Models;
using Lykke.Service.AdminManagement.Client.Models.Enums;
using Lykke.Service.AdminManagement.Client.Models.Requests;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class AdminsService : IAdminsService
    {
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;
        private readonly ICredentialsGeneratorService _credentialsGeneratorService;
        private readonly IMapper _mapper;
        
        public AdminsService(
            IAdminManagementServiceClient adminManagementServiceClient,
            IMapper mapper,
            ICredentialsGeneratorService credentialsGeneratorService)
        {
            _adminManagementServiceClient = adminManagementServiceClient;
            _mapper = mapper;
            _credentialsGeneratorService = credentialsGeneratorService;
        }

        public async Task<(AdminResetPasswordErrorCodes, Admin)> ResetPasswordAsync(string adminId)
        {
            var result = await _adminManagementServiceClient.AdminsApi.ResetPasswordAsync(
                    new ResetPasswordRequestModel
                    {
                        AdminUserId = adminId,
                        Password = await _credentialsGeneratorService.GenerateRandomPasswordForAdminAsync()
                    });

            switch (result.Error)
            {
                case ResetPasswordErrorCodes.None:
                    return (AdminResetPasswordErrorCodes.None, _mapper.Map<Admin>(result.Profile));
                case ResetPasswordErrorCodes.AdminUserNotFound:
                    return (AdminResetPasswordErrorCodes.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceCreateResponseError, Admin, string)> AuthenticateAsync(string email, string password)
        {
            var authenticationResult = await _adminManagementServiceClient.AuthApi.AuthenticateAsync(
                new AuthenticateRequestModel
                {
                    Email = email,
                    Password = password
                });

            switch (authenticationResult.Error)
            {
                case AdminManagementError.None:
                    var adminUserResponse = await _adminManagementServiceClient.AdminsApi.GetByEmailAsync(
                        new GetByEmailRequestModel { Email = email });

                    return (
                        AdminServiceCreateResponseError.None,
                        _mapper.Map<Admin>(adminUserResponse.Profile),
                        authenticationResult.Token);
                case AdminManagementError.AdminNotActive:
                    return (AdminServiceCreateResponseError.AdminNotActive, null, null);
                case AdminManagementError.LoginNotFound:
                    return (AdminServiceCreateResponseError.LoginNotFound, null, null);
                case AdminManagementError.PasswordMismatch:
                    return (AdminServiceCreateResponseError.PasswordMismatch, null, null);
                case AdminManagementError.InvalidEmailOrPasswordFormat:
                    return (AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat, null, null);
                default:
                    throw new InvalidOperationException($"Unexpected error during Authenticate for {email.SanitizeEmail()} - {authenticationResult.Error}");
            }
        }

        public async Task<AdminChangePasswordErrorCodes> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var changePasswordResult = await _adminManagementServiceClient.AuthApi.ChangePasswordAsync(
                new ChangePasswordRequestModel
                {
                    Email = email,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                });

            switch (changePasswordResult.Error)
            {
                case ChangePasswordErrorCodes.None:
                    return AdminChangePasswordErrorCodes.None;
                case ChangePasswordErrorCodes.LoginNotFound:
                    return AdminChangePasswordErrorCodes.LoginNotFound;
                case ChangePasswordErrorCodes.PasswordMismatch:
                    return AdminChangePasswordErrorCodes.PasswordMismatch;
                case ChangePasswordErrorCodes.InvalidEmailOrPasswordFormat:
                    return AdminChangePasswordErrorCodes.InvalidEmailOrPasswordFormat;
                case ChangePasswordErrorCodes.NewPasswordInvalid:
                    return AdminChangePasswordErrorCodes.NewPasswordInvalid;
                case ChangePasswordErrorCodes.AdminNotActive:
                    return AdminChangePasswordErrorCodes.AdminNotActive;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceCreateResponseError, Admin)> RegisterAsync(
            string email, string password,
            string phoneNumber, string firstName,
            string lastName, string company,
            string department, string jobTitle)
        {
            var createAdminResponse = await _adminManagementServiceClient.AdminsApi.RegisterAsync(
                new RegistrationRequestModel
                {
                    Company = company,
                    Department = department,
                    Email = email,
                    FirstName = firstName,
                    JobTitle = jobTitle,
                    LastName = lastName,
                    Password = password,
                    PhoneNumber = phoneNumber,
                    Permissions = new List<AdminPermission>
                    {
                        new AdminPermission { Level = AdminPermissionLevel.View, Type = PermissionType.Dashboard.ToString() }
                    }
                });

            switch (createAdminResponse.Error)
            {
                case AdminManagementError.None:
                    return (AdminServiceCreateResponseError.None, _mapper.Map<Admin>(createAdminResponse.Admin));
                case AdminManagementError.AlreadyRegistered:
                    return (AdminServiceCreateResponseError.AlreadyRegistered, null);
                case AdminManagementError.InvalidEmailOrPasswordFormat:
                    return (AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceResponseError, Admin)> GetAsync(string adminId)
        {
            var response = await _adminManagementServiceClient.AdminsApi.GetByIdAsync(
                new GetAdminByIdRequestModel
                {
                    AdminUserId = adminId
                });

            switch (response.Error)
            {
                case AdminUserResponseErrorCodes.None:
                    return (AdminServiceResponseError.None, _mapper.Map<Admin>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Simplified method for quick cache access, use only with existing admins.
        /// </summary>
        public async Task<bool> AdminHasPermissionAsync(string adminId, IReadOnlyList<PermissionType> types, PermissionLevel level)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return false;
            }
            
            var adminPermissions = await GetAdminPermissionsAsync(adminId);

            if (level == PermissionLevel.View)
            {
                return adminPermissions.Any(x =>
                        types.Contains(x.Type) && (x.Level == PermissionLevel.View || x.Level == PermissionLevel.Edit));
            }
            else
            {
                return adminPermissions.Any(x =>
                        types.Contains(x.Type) && x.Level == PermissionLevel.Edit);
            }
        }

        public async Task<(AdminServiceResponseError, Admin)> UpdateAdminAsync(
            string adminId, string phoneNumber,
            string firstName, string lastName,
            string company, string department,
            string jobTitle, bool isActive)
        {
            var response = await _adminManagementServiceClient.AdminsApi.UpdateAsync(
                new UpdateAdminRequestModel
                {
                    AdminUserId = adminId,
                    Company = company,
                    Department = department,
                    FirstName = firstName,
                    IsActive = isActive,
                    JobTitle = jobTitle,
                    LastName = lastName,
                    PhoneNumber = phoneNumber
                });

            switch (response.Error)
            {
                case AdminUserResponseErrorCodes.None:
                    return (AdminServiceResponseError.None, _mapper.Map<Admin>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public async Task<(AdminServiceResponseError, Admin)> UpdateAdminPermissionsAsync(string adminId, List<Permission> permissions)
        {
            var response = await _adminManagementServiceClient.AdminsApi.UpdatePermissionsAsync(
                new UpdatePermissionsRequestModel
                {
                    AdminUserId = adminId,
                    Permissions = permissions.Select(x => new AdminPermission
                    {
                        Level = _mapper.Map<AdminPermissionLevel>(x.Level),
                        Type = x.Type.ToString("G")
                    }).ToList()
                });
            
            switch (response.Error)
            {
                case AdminUserResponseErrorCodes.None:
                    return (AdminServiceResponseError.None, _mapper.Map<Admin>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(int, int, List<Admin>)> GetAsync(int pageSize, int pageNumber, string searchValue, bool? active)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                AdminUserResponseModel response = null;

                if (searchValue.IsValidEmail())
                    response = await _adminManagementServiceClient.AdminsApi.GetByEmailAsync(
                        new GetByEmailRequestModel
                        {
                            Email = searchValue,
                            Active = active
                        });
                
                return (
                    response?.Profile == null ? 0 : 1,
                    1,
                    response?.Profile == null
                        ? new List<Admin>()
                        : new List<Admin> { _mapper.Map<Admin>(response.Profile) });
            }

            var result = await _adminManagementServiceClient.AdminsApi.GetPaginatedAsync(
                new PaginationRequestModel
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    Active = active
                   
                });

            return (
                result.TotalCount,
                result.CurrentPage,
                _mapper.Map<List<Admin>>(result.AdminUsers));
        }

        public List<PermissionType> GetAllPermissions()
        {
            return Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>().ToList();
        }

        private async Task<List<Permission>> GetAdminPermissionsAsync(string adminId)
        {
            var adminPermissions = await _adminManagementServiceClient.AdminsApi.GetPermissionsAsync(
                new GetAdminByIdRequestModel
                {
                    AdminUserId = adminId
                });

            var permissions = adminPermissions != null
                ? adminPermissions.Select(x => new Permission
                {
                    Level = _mapper.Map<PermissionLevel>(x.Level),
                    Type = Enum.Parse<PermissionType>(x.Type)
                })
                : new List<Permission>();

            return permissions.ToList();
        }
    }
}
