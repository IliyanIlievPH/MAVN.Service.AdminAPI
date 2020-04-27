using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Service.AdminManagement.Client;
using MAVN.Service.AdminManagement.Client.Models;
using MAVN.Service.AdminManagement.Client.Models.Enums;
using MAVN.Service.AdminManagement.Client.Models.Requests;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.PartnerManagement.Client.Models;
using MAVN.Service.PartnerManagement.Client.Enums;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.ApiLibrary.Contract;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class AdminsService : IAdminsService
    {
        private const string DefaultStringValue = "-";
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;
        private readonly ICredentialsGeneratorService _credentialsGeneratorService;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IMapper _mapper;

        public AdminsService(
            IAdminManagementServiceClient adminManagementServiceClient,
            ICredentialsGeneratorService credentialsGeneratorService,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper)
        {
            _adminManagementServiceClient = adminManagementServiceClient;
            _mapper = mapper;
            _credentialsGeneratorService = credentialsGeneratorService;
            _partnerManagementClient = partnerManagementClient;
        }

        public async Task<(AdminResetPasswordErrorCodes, AdminModel)> ResetPasswordAsync(string adminId)
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
                    return (AdminResetPasswordErrorCodes.None, _mapper.Map<AdminModel>(result.Profile));
                case ResetPasswordErrorCodes.AdminUserNotFound:
                    return (AdminResetPasswordErrorCodes.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceCreateResponseError, AdminModel, string)> AuthenticateAsync(string email, string password)
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
                        _mapper.Map<AdminModel>(adminUserResponse.Profile),
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

        public async Task<(AdminServiceCreateResponseError, AdminModel)> RegisterPartnerAdminAsync(AdminRegisterModel model)
        {
            #region Create Admin

            var createAdminResponse = await _adminManagementServiceClient.AdminsApi.RegisterAsync(
                new RegistrationRequestModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    Company = model.CompanyName,
                    Localization = _mapper.Map<Localization>(model.Localization),
                    Department = DefaultStringValue,
                    FirstName = DefaultStringValue,
                    JobTitle = DefaultStringValue,
                    LastName = DefaultStringValue,
                    PhoneNumber = DefaultStringValue,
                    Permissions = new List<AdminPermission>
                    {
                        new AdminPermission
                        {
                            Type = PermissionType.Dashboard.ToString(),
                            Level = AdminPermissionLevel.View // TODO: set PartnerView
                        },
                        new AdminPermission
                        {
                            Type = PermissionType.ActionRules.ToString(),
                            Level = AdminPermissionLevel.View
                        },
                        new AdminPermission
                        {
                            Type = PermissionType.VoucherManager.ToString(),
                            Level = AdminPermissionLevel.Edit // TODO: set PartnerEdit
                        },
                        new AdminPermission
                        {
                            Type = PermissionType.ProgramPartners.ToString(),
                            Level = AdminPermissionLevel.Edit // TODO: set PartnerEdit
                        },
                        new AdminPermission
                        {
                            Type = PermissionType.AdminUsers.ToString(),
                            Level = AdminPermissionLevel.View // TODO: set PartnerEdit
                        },
                    }
                });

            if (createAdminResponse.Error != AdminManagementError.None)
            {
                switch (createAdminResponse.Error)
                {
                    case AdminManagementError.AlreadyRegistered:
                        return (AdminServiceCreateResponseError.AlreadyRegistered, null);
                    case AdminManagementError.InvalidEmailOrPasswordFormat:
                        return (AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat, null);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(createAdminResponse.Error), createAdminResponse.Error.ToString());
                }
            }

            #endregion

            #region Create Partner

            var partnerRequest = new PartnerCreateModel
            {
                Name = model.CompanyName,
                UseGlobalCurrencyRate = true,
                BusinessVertical = Vertical.Retail,
                ClientId = await _partnerManagementClient.Auth.GenerateClientId(),
                ClientSecret = await _partnerManagementClient.Auth.GenerateClientSecret(),
                CreatedBy = Guid.Parse(createAdminResponse.Admin.AdminUserId)
            };

            var response = await _partnerManagementClient.Partners.CreateAsync(partnerRequest);

            if (response.ErrorCode != PartnerManagementError.None)
                throw LykkeApiErrorException.BadRequest(new LykkeApiErrorCode(response.ErrorCode.ToString(), response.ErrorMessage));

            #endregion

            return (AdminServiceCreateResponseError.None, _mapper.Map<AdminModel>(createAdminResponse.Admin));
        }

        public async Task<(AdminServiceCreateResponseError, AdminModel)> RegisterAsync(
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
                        new AdminPermission
                        {
                            Level = AdminPermissionLevel.View,
                            Type = PermissionType.Dashboard.ToString()
                        }
                    }
                });

            switch (createAdminResponse.Error)
            {
                case AdminManagementError.None:
                    return (AdminServiceCreateResponseError.None, _mapper.Map<AdminModel>(createAdminResponse.Admin));
                case AdminManagementError.AlreadyRegistered:
                    return (AdminServiceCreateResponseError.AlreadyRegistered, null);
                case AdminManagementError.InvalidEmailOrPasswordFormat:
                    return (AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceResponseError, AdminModel)> GetAsync(string adminId)
        {
            var response = await _adminManagementServiceClient.AdminsApi.GetByIdAsync(
                new GetAdminByIdRequestModel
                {
                    AdminUserId = adminId
                });

            switch (response.Error)
            {
                case AdminUserResponseErrorCodes.None:
                    return (AdminServiceResponseError.None, _mapper.Map<AdminModel>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(AdminServiceResponseError, AdminModel)> UpdateAdminAsync(
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
                    return (AdminServiceResponseError.None, _mapper.Map<AdminModel>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public async Task<(AdminServiceResponseError, AdminModel)> UpdateAdminPermissionsAsync(string adminId, List<Domain.Models.Permission> permissions)
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
                    return (AdminServiceResponseError.None, _mapper.Map<AdminModel>(response.Profile));
                case AdminUserResponseErrorCodes.AdminUserDoesNotExist:
                    return (AdminServiceResponseError.AdminUserDoesNotExist, null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(int, int, List<AdminModel>)> GetAsync(int pageSize, int pageNumber, string searchValue, bool? active)
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
                        ? new List<AdminModel>()
                        : new List<AdminModel> { _mapper.Map<AdminModel>(response.Profile) });
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
                _mapper.Map<List<AdminModel>>(result.AdminUsers));
        }

        public List<PermissionType> GetAllPermissions()
        {
            return Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>().ToList();
        }
    }
}
