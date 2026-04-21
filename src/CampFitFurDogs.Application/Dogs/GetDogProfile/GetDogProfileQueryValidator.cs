using FluentValidation;
using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

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
