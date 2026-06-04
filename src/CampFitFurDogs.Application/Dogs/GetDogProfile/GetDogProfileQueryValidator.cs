using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using FluentValidation;
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public class GetDogProfileQueryValidator : AbstractValidator<GetDogProfileQuery>
{
    public GetDogProfileQueryValidator(ICurrentUserService currentUserService)
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerId).Equal(currentUserService.CurrentUserId);
        RuleFor(x => x.DogId).NotEmpty();
    }
}
