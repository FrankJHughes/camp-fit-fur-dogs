using Frank.Identity.Application.Abstractions.AuditLogging;

namespace CampFitFurDogs.TestUtilities.Fakes
{
    // ------------------------------------------------------------
    // Fake Audit Logger
    // ------------------------------------------------------------
    public sealed class FakeAuditLogger : IAuditLogger
    {
        public Exception? ExceptionToThrow { get; set; }

        public Guid? CapturedUserId { get; private set; }
        public string? CapturedExternalId { get; private set; }

        public Task LoginSucceeded(Guid userId, string externalId)
        {
            if (ExceptionToThrow is not null)
                throw ExceptionToThrow;

            CapturedUserId = userId;
            CapturedExternalId = externalId;

            return Task.CompletedTask;
        }
    }
}
