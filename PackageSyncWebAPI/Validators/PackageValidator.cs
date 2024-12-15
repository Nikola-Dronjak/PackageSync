using FluentValidation;
using PackageSyncWebAPI.Models;

namespace PackageSyncWebAPI.Validators
{
    public class PackageValidator : AbstractValidator<Package>
    {
        public PackageValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 255).WithMessage("Name must be between 3 and 255 characters.");

            RuleFor(p => p.DateOfDelivery)
                .GreaterThan(DateTime.Now).WithMessage("Date of delivery must be in the future.");
        }
    }
}
