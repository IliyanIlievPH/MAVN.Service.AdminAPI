using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Settings
{
    [PublicAPI]
    public class OperationFeesModel
    {
        public Money18? CrossChainTransferFee { get; set; }
        public Money18? FirstTimeLinkingFee { get; set; }
        public Money18? SubsequentLinkingFee { get; set; }
    }
}
