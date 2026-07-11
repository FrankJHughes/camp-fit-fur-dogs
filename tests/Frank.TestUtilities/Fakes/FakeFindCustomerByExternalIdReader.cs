using System;
using System.Threading;
using System.Threading.Tasks;
using Frank.Application.Abstractions.Users.FindUserByExternalId;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeFindUserByExternalIdReader : IFindUserByExternalIdReader
{
    public FindUsererByExternalIdResponse? Response { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<FindUsererByExternalIdResponse?> FindByExternalIdAsync(string externalAuthProviderId, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Response);
    }
}
