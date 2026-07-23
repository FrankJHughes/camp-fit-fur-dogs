using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Sessions.RevokeSession;

public sealed class RevokeSessionHandler : ICommandHandler<RevokeSessionCommand>
{
    private readonly IRevokeSessionWriter _writer;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    public RevokeSessionHandler(IRevokeSessionWriter writer, IFrankIdentityUnitOfWork unitOfWork)
    {
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(RevokeSessionCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = SessionTokenHash.From(command.TokenHash);

        await _writer.WriteAsync(tokenHash, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
