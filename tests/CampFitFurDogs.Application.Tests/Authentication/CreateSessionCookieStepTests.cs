using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;

namespace CampFitFurDogs.Application.Tests.Authentication
{
    public sealed class CreateSessionCookieStepTests
    {
        private static AuthCallbackContext ContextWithCustomerId(Guid id) =>
            new AuthCallbackContext("dummy-code")
            {
                CustomerId = id
            };

        private static CreateSessionCookieStep CreateStep() => new CreateSessionCookieStep();

        // ------------------------------------------------------------
        // SUCCESSFUL RESULT CREATION
        // ------------------------------------------------------------
        [Fact]
        public async Task Sets_result_to_success_with_customer_id()
        {
            var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var ctx = ContextWithCustomerId(customerId);

            var step = CreateStep();

            await step.ExecuteAsync(ctx, CancellationToken.None);

            ctx.Result.Should().NotBeNull();
            ctx.Result!.CustomerId.Should().Be(customerId);
            ctx.Result.SessionCookie.Should().Be($"cfd.session={customerId}");
            ctx.Result.RedirectUrl.Should().Be("");
        }

        // ------------------------------------------------------------
        // NULL CUSTOMER ID → NULL REFERENCE EXCEPTION
        // ------------------------------------------------------------
        [Fact]
        public async Task Null_customer_id_throws_InvalidOperationException()
        {
            var ctx = new AuthCallbackContext("dummy-code")
            {
                CustomerId = null
            };

            var step = new CreateSessionCookieStep();

            var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
