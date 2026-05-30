using CampFitFurDogs.Application.Abstractions.Audit;

namespace CampFitFurDogs.TestUtilities.Fakes
{
    // ------------------------------------------------------------
    // Fake Audit Logger
    // ------------------------------------------------------------
    public sealed class FakeAuditLogger : IAuditLogger
    {
        public Exception? ExceptionToThrow { get; set; }

        public Guid? CapturedCustomerId { get; private set; }
        public string? CapturedExternalId { get; private set; }

        public Task LoginSucceeded(Guid customerId, string externalId)
        {
            if (ExceptionToThrow is not null)
                throw ExceptionToThrow;

            CapturedCustomerId = customerId;
            CapturedExternalId = externalId;

            return Task.CompletedTask;
        }
    }
}
