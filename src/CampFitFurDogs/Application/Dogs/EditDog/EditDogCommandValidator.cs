using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Domain.Dogs;
using FluentValidation;

namespace CampFitFurDogs.Application.Dogs.EditDog;

public class EditDogCommandValidator : AbstractValidator<EditDogCommand>
{
    public EditDogCommandValidator()
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
