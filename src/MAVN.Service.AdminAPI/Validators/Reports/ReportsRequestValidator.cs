using FluentValidation;
using MAVN.Service.AdminAPI.Models.Reports;

namespace MAVN.Service.AdminAPI.Validators.Reports
{
    public class ReportsRequestValidator 
        : AbstractValidator<ReportRequestModel>
    {
        public ReportsRequestValidator()
        {
            RuleFor(r => r.From)
                .NotEmpty();
            RuleFor(r => r.To)
                .NotEmpty();
        }
    }
}
