using System.Linq;
using FluentValidation;
using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.BurnRules
{
    public abstract class BurnRuleBaseRequestValidator<T, TU> : AbstractValidator<T>
        where T : BurnRuleBaseRequest<TU> where TU : IMobileContentRequest
    {
        protected BurnRuleBaseRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Title)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Title should be specified with length between 3 and 50 characters");

            RuleFor(o => o.Description)
                .Must(x => string.IsNullOrEmpty(x) || x.Length >= 3 && x.Length <= 1000)
                .WithMessage("Description should be specified with length between 3 and 1000 characters");

            RuleFor(p => p.BusinessVertical)
                .NotNull()
                .WithMessage("Business vertical required");

            Include(new ConversionRateValidator());

            RuleFor(o => o.PartnerIds)
                .Must(p => p == null || p.GroupBy(n => n).All(c => c.Count() == 1))
                .WithMessage("Partner IDs must not have duplicates.");

            RuleFor(o => o.MobileContents)
                .Must(contents => contents != null && contents.Any())
                .WithMessage(o => $"There should be at least one item in the {nameof(o.MobileContents)} value")
                .Must(contents => { return contents.Any(c => c.MobileLanguage == Localization.En); })
                .WithMessage("English content is required.");
        }
    }
}
