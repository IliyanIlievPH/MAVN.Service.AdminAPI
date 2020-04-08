namespace MAVN.Service.AdminAPI.Models.Customers
{
    public enum CustomerOperationTransactionType
    {
        P2P,
        Earn,
        Burn,
        BurnCancelled,
        ReferralStake,
        ReleasedReferralStake,
        LinkedWalletTransfer,
        FeeCollected
    }
}
