namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents a response to generating random password as a suggestion.
    /// </summary>
    public class GeneratedPasswordModel
    {
        /// <summary>
        /// Generated random password.
        /// </summary>
        public string Password { set; get; }
    }
}
