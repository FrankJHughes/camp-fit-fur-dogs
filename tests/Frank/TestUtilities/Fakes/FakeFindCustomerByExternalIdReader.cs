using Frank.Identity.Application.Abstractions.Users.FindUserByExternalId;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeFindUserByExternalIdReader : IFindUserByExternalIdReader
{
    public FindUserByExternalIdResponse? Response { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<FindUserByExternalIdResponse?> FindByExternalIdAsync(string externalAuthProviderId, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Response);
    }
}
