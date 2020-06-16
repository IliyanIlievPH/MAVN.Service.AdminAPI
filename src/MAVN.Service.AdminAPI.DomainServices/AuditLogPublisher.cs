using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AuditLogs.Contract.Events;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class AuditLogPublisher : IAuditLogPublisher
    {
        private readonly IRabbitPublisher<AuditLogEvent> _publisher;

        public AuditLogPublisher(IRabbitPublisher<AuditLogEvent> publisher)
        {
            _publisher = publisher;
        }

        public Task PublishAuditLogAsync(string adminId, string jsonContext, ActionType actionType)
        {
            return _publisher.PublishAsync(new AuditLogEvent
            {
                AdminUserId = Guid.Parse(adminId),
                ActionContextJson = jsonContext,
                ActionType = actionType.ToString(),
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
