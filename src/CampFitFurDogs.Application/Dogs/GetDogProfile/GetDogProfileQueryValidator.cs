using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using FluentValidation;
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public class GetDogProfileQueryValidator : AbstractValidator<GetDogProfileQuery>
{
    public GetDogProfileQueryValidator(ICurrentUser currentUserService)
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerId).Equal(currentUserService.Id);
        RuleFor(x => x.DogId).NotEmpty();
    }
}
