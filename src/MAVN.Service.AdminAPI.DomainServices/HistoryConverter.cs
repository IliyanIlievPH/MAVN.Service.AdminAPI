using System;
using System.Collections.Generic;
using System.Linq;
using MAVN.Numerics;
using MAVN.Service.OperationsHistory.Client.Models.Responses;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class HistoryConverter : IHistoryConverter
    {
        private readonly string _tokenSymbol;

        public HistoryConverter(string tokenSymbol)
        {
            _tokenSymbol = tokenSymbol;
        }

        public IEnumerable<CustomerOperation> FromTransfers(string customerId, IEnumerable<TransferResponse> source)
        {
            return source.Select(item => new CustomerOperation
            {
                Timestamp = item.Timestamp,
                PartnerId = item.TransactionId,
                TransactionId = item.TransactionId,
                TransactionType = CustomerOperationTransactionType.P2P,
                WalletAddress =
                    item.ReceiverCustomerId == customerId ? item.WalletAddress : item.OtherSideWalletAddress,
                ReceiverCustomerId = item.ReceiverCustomerId,
                Amount = item.ReceiverCustomerId == customerId
                    ? Money18.Abs(item.Amount)
                    : -1 * Money18.Abs(item.Amount),
                AssetSymbol = item.AssetSymbol
            });
        }

        public IEnumerable<CustomerOperation> FromBonusCashIns(IEnumerable<BonusCashInResponse> source)
        {
            return source.Select(item => new CustomerOperation
            {
                Timestamp = item.Timestamp,
                PartnerId = item.PartnerId ?? item.TransactionId,
                TransactionId = item.TransactionId,
                TransactionType = CustomerOperationTransactionType.Earn,
                CampaignName = item.CampaignName,
                Amount = item.Amount,
                AssetSymbol = item.AssetSymbol
            });
        }


        public IEnumerable<CustomerOperation> FromPartnersPayments(IEnumerable<PartnersPaymentResponse> source)
        {
            return source.Select(item => new CustomerOperation
            {
                Timestamp = item.Timestamp,
                PartnerId = item.PartnerId,
                TransactionId = item.PaymentRequestId,
                TransactionType = CustomerOperationTransactionType.Burn,
                Amount = -1 * Money18.Abs(item.Amount),
                AssetSymbol = _tokenSymbol
            });
        }

        public IEnumerable<CustomerOperation> FromRefundedPartnersPayments(string customerId,
            IEnumerable<PartnersPaymentResponse> source)
        {
            return source.Select(item => new CustomerOperation
            {
                Timestamp = item.Timestamp,
                PartnerId = item.PartnerId,
                TransactionId = item.PaymentRequestId,
                ReceiverCustomerId = customerId,
                TransactionType = CustomerOperationTransactionType.BurnCancelled,
                Amount = Money18.Abs(item.Amount),
                AssetSymbol = _tokenSymbol
            });
        }

        public IEnumerable<CustomerOperation> FromReferralStakes(IEnumerable<ReferralStakeResponse> source)
        {
            if (source != null)
            {
                return source.Select(item => new CustomerOperation
                {
                    Timestamp = item.Timestamp,
                    PartnerId = item.ReferralId,
                    TransactionId = item.ReferralId,
                    TransactionType = CustomerOperationTransactionType.ReferralStake,
                    ReceiverCustomerId = item.CustomerId,
                    CampaignName = item.CampaignName,
                    Amount = Money18.Abs(item.Amount),
                    AssetSymbol = item.AssetSymbol
                });
            }

            return new List<CustomerOperation>();
        }

        public IEnumerable<CustomerOperation> FromReleasedReferralStakes(IEnumerable<ReferralStakeResponse> source)
        {
            if (source != null)
            {
                return source.Select(item => new CustomerOperation
                {
                    Timestamp = item.Timestamp,
                    PartnerId = item.ReferralId,
                    TransactionId = item.ReferralId,
                    TransactionType = CustomerOperationTransactionType.ReleasedReferralStake,
                    ReceiverCustomerId = item.CustomerId,
                    CampaignName = item.CampaignName,
                    Amount = Money18.Abs(item.Amount),
                    AssetSymbol = item.AssetSymbol
                });
            }

            return new List<CustomerOperation>();
        }

        public IEnumerable<CustomerOperation> FromVoucherPurchasePayments(
            IEnumerable<VoucherPurchasePaymentResponse> source, IReadOnlyDictionary<Guid, string> spendRuleNames)
        {
            if (source != null)
            {
                return source.Select(item => new CustomerOperation
                {
                    Timestamp = item.Timestamp,
                    TransactionId = item.TransferId.ToString(),
                    TransactionType = CustomerOperationTransactionType.VoucherPurchasePayment,
                    CampaignName = spendRuleNames.ContainsKey(item.SpendRuleId)
                        ? spendRuleNames[item.SpendRuleId]
                        : null,
                    Amount = -Money18.Abs(item.Amount),
                    AssetSymbol = item.AssetSymbol
                });
            }

            return new List<CustomerOperation>();
        }

        public IEnumerable<CustomerOperation> FromLinkedWalletTransfers(string customerId, IEnumerable<LinkedWalletTransferResponse> source)
        {
            if (source != null)
            {
                return source.Select(item => new CustomerOperation
                {
                    Timestamp = item.Timestamp,
                    TransactionId = item.TransactionId,
                    TransactionType = CustomerOperationTransactionType.LinkedWalletTransfer,
                    WalletAddress = item.Direction == LinkedWalletTransferDirection.Incoming ? item.LinkedWalletAddress : item.WalletAddress,
                    ReceiverCustomerId = customerId,
                    Amount = item.Direction == LinkedWalletTransferDirection.Outgoing ? Money18.Abs(item.Amount) * -1 : Money18.Abs(item.Amount),
                    AssetSymbol = item.AssetSymbol
                });
            }

            return new List<CustomerOperation>();
        }

        public IEnumerable<CustomerOperation> FromFeeCollectedOperations(IEnumerable<FeeCollectedOperationResponse> source)
        {
            if (source != null)
            {
                return source.Select(item => new CustomerOperation
                {
                    Timestamp = item.Timestamp,
                    TransactionId = item.OperationId,
                    TransactionType = CustomerOperationTransactionType.FeeCollected,
                    ReceiverCustomerId = item.CustomerId,
                    Amount = Money18.Abs(item.Fee),
                    AssetSymbol = item.AssetSymbol
                });
            }

            return new List<CustomerOperation>();
        }
    }
}
