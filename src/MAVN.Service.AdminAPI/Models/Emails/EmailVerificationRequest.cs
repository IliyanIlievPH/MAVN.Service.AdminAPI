namespace MAVN.Service.AdminAPI.Models.Emails
{
    /// <summary>
    /// Email verification request
    /// </summary>
    public class EmailVerificationRequest
    {
        /// <summary>
        /// Verification code
        /// </summary>
        public string VerificationCode { get; set; }
    }
}
