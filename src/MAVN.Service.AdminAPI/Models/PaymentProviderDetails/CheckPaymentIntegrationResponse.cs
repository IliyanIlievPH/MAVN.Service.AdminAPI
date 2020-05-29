namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    public class CheckPaymentIntegrationResponse
    {
        public bool IsConfiguredCorrectly { get; set; }

        public string Error { get; set; }
    }
}
