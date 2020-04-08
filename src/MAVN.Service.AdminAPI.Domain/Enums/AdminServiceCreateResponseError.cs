namespace MAVN.Service.AdminAPI.Domain.Enums
{
    public enum AdminServiceCreateResponseError
    {
        None,
        LoginNotFound,
        PasswordMismatch,
        RegisteredWithAnotherPassword,
        AlreadyRegistered,
        InvalidEmailOrPasswordFormat,
        AdminNotActive
    }
}
