namespace MAVN.Service.AdminAPI.Domain.Enums
{
    public enum AdminServiceCreateResponseError
    {
        None,
        LoginNotFound,
        AdminEmailIsNotVerified,
        PasswordMismatch,
        RegisteredWithAnotherPassword,
        AlreadyRegistered,
        InvalidEmailOrPasswordFormat,
        AdminNotActive
    }
}
