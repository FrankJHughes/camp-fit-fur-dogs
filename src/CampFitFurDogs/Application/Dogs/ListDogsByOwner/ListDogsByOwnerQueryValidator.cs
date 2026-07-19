using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using FluentValidation;
using Frank.Identity.Application.Abstractions;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

public class ListDogsByOwnerQueryValidator : AbstractValidator<ListDogsByOwnerQuery>
{
    public ListDogsByOwnerQueryValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.OwnerId).Equal(currentUser.Id!.Value);
    }
}
