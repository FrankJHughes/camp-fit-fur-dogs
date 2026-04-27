using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Tests.Fakes;

namespace CampFitFurDogs.Application.Tests.Dogs.ListDogsByOwner;

public class ListDogsByOwnerQueryValidatorTests
{
    [Fact]
    public async Task Validate_OwnerIdMatchesCurrentUser_IsValid()
    {
        var userId = Guid.NewGuid();
        var currentUser = new FakeCurrentUserService(userId);
        var validator = new ListDogsByOwnerQueryValidator(currentUser);

        var query = new ListDogsByOwnerQuery(userId);
        var result = await validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_OwnerIdDoesNotMatchCurrentUser_IsInvalid()
    {
        var currentUser = new FakeCurrentUserService(Guid.NewGuid());
        var validator = new ListDogsByOwnerQueryValidator(currentUser);

        var query = new ListDogsByOwnerQuery(Guid.NewGuid());
        var result = await validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_OwnerIdIsEmpty_IsInvalid()
    {
        var currentUser = new FakeCurrentUserService(Guid.Empty);
        var validator = new ListDogsByOwnerQueryValidator(currentUser);

        var query = new ListDogsByOwnerQuery(Guid.Empty);
        var result = await validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
    }
}
