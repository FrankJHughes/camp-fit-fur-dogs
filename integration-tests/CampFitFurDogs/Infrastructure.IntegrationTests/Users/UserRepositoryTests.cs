using CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Users;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Can_Create_And_Retrieve_User()
    {
        var user = User.Create(
            firstName: FirstName.From("Test"),
            lastName: LastName.From("User"),
            email: Email.From("test@example.com"),
            externalId: ExternalId.From("auth0|1234567890"),
            phone: PhoneNumber.From("916-555-5555")
        );

        var users = _fixture.DbContext.Set<User>();
        users.Add(user);
        await _fixture.DbContext.SaveChangesAsync();

        var loaded = await users.FindAsync(user.Id);

        Assert.NotNull(loaded);
        Assert.Equal(FirstName.From("Test"), loaded!.FirstName);
        Assert.Equal(ExternalId.From("auth0|1234567890"), loaded.ExternalId);
    }
}
