using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.AdminAPI.Settings.Service
{
    public class RabbitMqSettings
    {
        public RabbitMqExchangeSettings Publishers { get; set; }
    }

    public class RabbitMqExchangeSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
    }
}
