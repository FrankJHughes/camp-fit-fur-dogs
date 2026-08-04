using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.GetDog;

public class GetDogQueryValidator : AbstractValidator<GetDogQuery>
{
    public GetDogQueryValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.OwnerId).Equal(currentUser.Id!.Value);
        RuleFor(x => x.DogId).NotEmpty();
    }
}
