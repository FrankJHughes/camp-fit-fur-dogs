using FluentValidation;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Dogs.EditDogProfile;

public class EditDogProfileCommandValidator : AbstractValidator<EditDogProfileCommand>
{
    public EditDogProfileCommandValidator()
    {
        RuleFor(x => x.DogId).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Breed).NotEmpty();
        // RuleFor(x => x.Sex).NotEmpty();
        RuleFor(x => x.Sex)
            .Must(s => Enum.TryParse<Sex>(s, out _))
            .WithMessage("Sex must be 'Male' or 'Female'.");
    }
}
