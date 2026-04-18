using FluentValidation;

using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

public class RegisterDogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    public RegisterDogCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Breed)
            .NotEmpty();

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.Sex)
            .NotEmpty()
            .Must(s => s.Equals("Male", StringComparison.OrdinalIgnoreCase)
                    || s.Equals("Female", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sex must be 'Male' or 'Female'.");
    }
}
