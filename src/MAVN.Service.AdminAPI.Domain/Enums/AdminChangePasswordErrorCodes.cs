namespace MAVN.Service.AdminAPI.Domain.Enums
{
    public enum AdminChangePasswordErrorCodes
    {
        None,
        LoginNotFound,
        PasswordMismatch,
        InvalidEmailOrPasswordFormat,
        NewPasswordInvalid,
        AdminNotActive
    }
}
