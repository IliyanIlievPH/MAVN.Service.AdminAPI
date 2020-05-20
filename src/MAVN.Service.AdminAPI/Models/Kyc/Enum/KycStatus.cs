namespace MAVN.Service.AdminAPI.Models.Kyc.Enum
{
    /// <summary>Holds possible statuses for KYC</summary>
    public enum KycStatus
    {
        /// <summary>Pending status</summary>
        Pending,
        /// <summary>In review status</summary>
        InReview,
        /// <summary>Rejected status</summary>
        Rejected,
        /// <summary>Accepted status</summary>
        Accepted,
        /// <summary>Requires data status</summary>
        RequiresData,
    }
}
