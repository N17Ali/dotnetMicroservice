using FluentValidation;

namespace PlatformsService.Dtos.Validation;

public class PlatformCreateDtoValidator : AbstractValidator<PlatformCreateDto>
{
    public PlatformCreateDtoValidator()
    {
        RuleFor(platform => platform.Name)
            .NotEmpty().WithMessage("Platform name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(platform => platform.Publisher)
            .NotEmpty().WithMessage("Publisher is required.")
            .Length(2, 100).WithMessage("Publisher must be between 2 and 100 characters.");

        RuleFor(platform => platform.Cost)
            .NotEmpty().WithMessage("Cost is required.");
        // .Must(BeAValidDecimal).WithMessage("Cost must be a valid numeric value."); // Custom validation rule

        // custom validation method
        // bool BeAValidDecimal(string costString)
        // {
        //     return decimal.TryParse(costString, out _);
        // }
    }
}