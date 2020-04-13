using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface IAdminsService
    {
        Task<(AdminServiceCreateResponseError, Admin, string)> AuthenticateAsync(string email, string password);

        Task<AdminChangePasswordErrorCodes> ChangePasswordAsync(
            string email,
            string currentPassword,
            string newPassword);

        Task<(AdminServiceCreateResponseError, Admin)> RegisterAsync(
            string email,
            string password,
            string phoneNumber,
            string firstName,
            string lastName,
            string company,
            string department,
            string jobTitle);

        Task<(AdminServiceResponseError, Admin)> UpdateAdminAsync(
            string adminId,
            string phoneNumber,
            string firstName,
            string lastName,
            string company,
            string department,
            string jobTitle,
            bool isActive);
        
        Task<(AdminServiceResponseError, Admin)> UpdateAdminPermissionsAsync(string adminId, List<Permission> permissions);
        
        Task<(int, int, List<Admin>)> GetAsync(
            int pageSize,
            int pageNumber,
            string searchValue,
            bool? active);

        Task<(AdminServiceResponseError, Admin)> GetAsync(string adminId);

        List<PermissionType> GetAllPermissions();

        Task<(AdminResetPasswordErrorCodes, Admin)> ResetPasswordAsync(string adminId);
    }
}
