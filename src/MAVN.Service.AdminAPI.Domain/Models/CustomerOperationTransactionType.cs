namespace MAVN.Service.AdminAPI.Domain.Models
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
        FeeCollected,
        VoucherPurchasePayment
    }
}
