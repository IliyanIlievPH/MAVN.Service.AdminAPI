using System;
using System.Collections.Generic;
using MAVN.Service.OperationsHistory.Client.Models.Responses;
using MAVN.Service.AdminAPI.Domain.Models;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface IHistoryConverter
    {
        IEnumerable<CustomerOperation> FromTransfers(string customerId, IEnumerable<TransferResponse> source);
        IEnumerable<CustomerOperation> FromBonusCashIns(IEnumerable<BonusCashInResponse> source);
        IEnumerable<CustomerOperation> FromPartnersPayments(IEnumerable<PartnersPaymentResponse> source);
        IEnumerable<CustomerOperation> FromRefundedPartnersPayments(string customerId, IEnumerable<PartnersPaymentResponse> source);
        IEnumerable<CustomerOperation> FromReferralStakes(IEnumerable<ReferralStakeResponse> source);
        IEnumerable<CustomerOperation> FromReleasedReferralStakes(IEnumerable<ReferralStakeResponse> source);
        IEnumerable<CustomerOperation> FromLinkedWalletTransfers(string customerId, IEnumerable<LinkedWalletTransferResponse> source);
        IEnumerable<CustomerOperation> FromFeeCollectedOperations(IEnumerable<FeeCollectedOperationResponse> source);
        IEnumerable<CustomerOperation> FromVoucherPurchasePayments(IEnumerable<VoucherPurchasePaymentResponse> source,
            IReadOnlyDictionary<Guid, string> spendRuleNames);
    }
}
