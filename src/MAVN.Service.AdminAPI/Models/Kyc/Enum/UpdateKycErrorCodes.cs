namespace MAVN.Service.AdminAPI.Models.Kyc.Enum
{
    /// <summary>Holds error codes</summary>
    public enum UpdateKycErrorCodes
    {
        /// <summary>No error</summary>
        None,
        /// <summary>Kyc does not exist</summary>
        KycDoesNotExist,
        /// <summary>Comment is required</summary>
        CommentRequired,
        /// <summary>Cannot change to this new status from the current one</summary>
        InvalidStatus,
    }
}
