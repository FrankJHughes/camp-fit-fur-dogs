using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using FluentValidation;
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

public class ListDogsByOwnerQueryValidator : AbstractValidator<ListDogsByOwnerQuery>
{
    public ListDogsByOwnerQueryValidator(ICurrentUserService currentUserService)
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.OwnerId).Equal(currentUserService.CurrentUserId);
    }
}
