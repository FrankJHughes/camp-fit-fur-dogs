using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public class GetDogProfileQueryValidator : AbstractValidator<GetDogProfileQuery>
{
    public GetDogProfileQueryValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.OwnerId).Equal(currentUser.Id!.Value);
        RuleFor(x => x.DogId).NotEmpty();
    }
}
