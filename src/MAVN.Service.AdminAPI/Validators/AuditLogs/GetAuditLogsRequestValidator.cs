using FluentValidation;
using MAVN.Service.AdminAPI.Models.AuditLogs;

namespace MAVN.Service.AdminAPI.Validators.AuditLogs
{
    public class GetAuditLogsRequestValidator : AbstractValidator<GetAuditLogsRequest>
    {
        public GetAuditLogsRequestValidator()
        {
            RuleFor(x => x.FromDate)
                .NotNull()
                .NotEmpty()
                .When(x => x.ToDate.HasValue && x.ToDate.Value != default);

            RuleFor(x => x.ToDate)
                .NotNull()
                .NotEmpty()
                .GreaterThan(x => x.FromDate)
                .When(x => x.FromDate.HasValue && x.FromDate.Value != default);
        }
    }
}
