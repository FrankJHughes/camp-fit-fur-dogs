using CampFitFurDogs.Application.Abstractions.Identity.External;

namespace CampFitFurDogs.Api.Tests.Fakes;

public sealed class FakeExternalIdentityResolver : IExternalIdentityResolver
{
    private readonly Guid _result;

    public FakeExternalIdentityResolver(Guid result)
    {
        _result = result;
    }

    public string? LastExternalId { get; private set; }
    public string? LastFirstName { get; private set; }
    public string? LastLastName { get; private set; }
    public string? LastEmail { get; private set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<Guid> ResolveAsync(
        string externalId,
        string firstName,
        string lastName,
        string email,
        CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        LastExternalId = externalId;
        LastFirstName = firstName;
        LastLastName = lastName;
        LastEmail = email;

        return Task.FromResult(_result);
    }
}
