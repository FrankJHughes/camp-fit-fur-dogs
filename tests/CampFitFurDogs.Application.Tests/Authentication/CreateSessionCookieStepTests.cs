using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Customers;

public sealed class CreateSessionCookieStepTests
{
    [Fact]
    public async Task Generates_token_hash_and_cookie_and_result()
    {
        var customerId = Guid.NewGuid();

        var ctx = new AuthCallbackContext("code")
        {
            CustomerId = customerId
        };

        var step = new CreateSessionCookieStep();

        await step.ExecuteAsync(ctx, CancellationToken.None);

        ctx.TokenHash.Should().NotBeNull();
        ctx.TokenHash!.Value.Length.Should().Be(64);

        ctx.Result.Should().NotBeNull();
        ctx.Result!.CustomerId.Should().Be(CustomerId.From(customerId));

        ctx.Result.Cookie.Name.Should().Be("cfd.session");
        ctx.Result.Cookie.Value.Should().HaveLength(64); // plaintext token

        ctx.Result.RedirectUrl.Should().Be("");
    }

    [Fact]
    public async Task Missing_customerId_throws()
    {
        var ctx = new AuthCallbackContext("code");

        var step = new CreateSessionCookieStep();

        Func<Task> act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
