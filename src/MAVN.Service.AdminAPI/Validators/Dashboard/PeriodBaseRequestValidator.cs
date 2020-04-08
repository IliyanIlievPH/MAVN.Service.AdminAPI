using System;
using FluentValidation;
using MAVN.Service.AdminAPI.Models.Dashboard;

namespace MAVN.Service.AdminAPI.Validators.Dashboard
{
    public abstract class PeriodBaseRequestValidator<T> : AbstractValidator<T>
        where T : BasePeriodRequest
    {
        protected PeriodBaseRequestValidator()
        {
            RuleFor(o => o.FromDate.Date)
                .NotEmpty()
                .WithMessage("From Date is required")
                .LessThanOrEqualTo(x => DateTime.UtcNow.Date)
                .WithMessage("From Date must be equal or earlier than today.");

            RuleFor(o => o.ToDate.Date)
                .NotEmpty()
                .WithMessage("To Date is required")
                .GreaterThanOrEqualTo(x => x.FromDate.Date)
                .WithMessage("To Date must be equal or later than From Date.")
                .LessThanOrEqualTo(x => DateTime.UtcNow.Date)
                .WithMessage("To Date must be equal or earlier than today.");
        }
    }
}
