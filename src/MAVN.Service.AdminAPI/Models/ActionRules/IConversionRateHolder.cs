using Falcon.Numerics;

namespace MAVN.Service.AdminAPI.Models.ActionRules
{
    public interface IConversionRateHolder
    {
        Money18? AmountInTokens { get; set; }
        decimal? AmountInCurrency { get; set; }
        bool UsePartnerCurrencyRate { get; set; }
    }
}
