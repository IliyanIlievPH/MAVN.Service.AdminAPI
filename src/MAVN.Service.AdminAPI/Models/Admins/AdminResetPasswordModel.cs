namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents a request to rest admin password
    /// </summary>
    public class AdminResetPasswordModel
    {
        /// <summary>
        /// Id of the admin
        /// </summary>
        public string AdminId { set; get; }
    }
}
