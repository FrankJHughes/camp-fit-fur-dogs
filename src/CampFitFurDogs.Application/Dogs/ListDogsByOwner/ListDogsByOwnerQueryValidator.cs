using FluentValidation;
using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

public class ListDogsByOwnerQueryValidator : AbstractValidator<ListDogsByOwnerQuery>
{
    public ListDogsByOwnerQueryValidator(ICurrentUserService currentUserService)
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.OwnerId).Equal(currentUserService.CurrentUserId);
    }
}
