using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication
{
    public sealed partial class AuditLoginStepTests
    {

        private static AuthCallbackContext ContextWithProfileAndCustomer(Guid customerId)
        {
            return new AuthCallbackContext("dummy-code")
            {
                User = new AuthUser(
                    ExternalId: "auth0|abc",
                    GivenName: "Frank",
                    FamilyName: "Smith",
                    Email: "frank@example.com"),
                CustomerId = customerId
            };
        }

        private static AuditLoginStep CreateStep(FakeAuditLogger fake)
            => new AuditLoginStep(fake);

        // ------------------------------------------------------------
        // SUCCESSFUL AUDIT LOGGING
        // ------------------------------------------------------------
        [Fact]
        public async Task Calls_audit_logger_with_correct_parameters()
        {
            var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var ctx = ContextWithProfileAndCustomer(customerId);

            var fake = new FakeAuditLogger();
            var step = CreateStep(fake);

            await step.ExecuteAsync(ctx, CancellationToken.None);

            fake.CapturedCustomerId.Should().Be(customerId);
            fake.CapturedExternalId.Should().Be("auth0|abc");
        }

        // ------------------------------------------------------------
        // LOGGER EXCEPTION BUBBLES UP
        // ------------------------------------------------------------
        [Fact]
        public async Task Logger_exception_bubbles_up()
        {
            var customerId = Guid.NewGuid();
            var ctx = ContextWithProfileAndCustomer(customerId);

            var fake = new FakeAuditLogger
            {
                ExceptionToThrow = new InvalidOperationException("Audit failed")
            };

            var step = CreateStep(fake);

            var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Audit failed");
        }

        // ------------------------------------------------------------
        // NULL PROFILE → NULL REFERENCE EXCEPTION
        // ------------------------------------------------------------
        [Fact]
        public async Task Null_profile_throws_NullReferenceException()
        {
            var ctx = new AuthCallbackContext("dummy-code")
            {
                User = null,
                CustomerId = Guid.NewGuid()
            };

            var fake = new FakeAuditLogger();
            var step = CreateStep(fake);

            var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }

        // ------------------------------------------------------------
        // NULL CUSTOMER ID → INVALID OPERATION EXCEPTION
        // ------------------------------------------------------------
        [Fact]
        public async Task Null_customer_id_throws_InvalidOperationException()
        {
            var ctx = new AuthCallbackContext("dummy-code")
            {
                User = new AuthUser(
                    ExternalId: "auth0|abc",
                    GivenName: "Frank",
                    FamilyName: "Smith",
                    Email: "frank@example.com"),
                CustomerId = null
            };

            var fake = new FakeAuditLogger();
            var step = CreateStep(fake);

            var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

            // Guid?.Value throws InvalidOperationException when null
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
