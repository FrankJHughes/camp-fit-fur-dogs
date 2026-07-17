using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeFindUserByExternalIdReader : IGetUserByExternalIdReader
{
    public GetUserByExternalIdResponse? Response { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<GetUserByExternalIdResponse?> GetByExternalIdAsync(string externalAuthProviderId, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Response);
    }
}
