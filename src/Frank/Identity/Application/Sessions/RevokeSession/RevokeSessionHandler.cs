using Frank.Core.Application.Abstractions.Command;
using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Sessions.RevokeSession;

public sealed class RevokeSessionHandler : ICommandHandler<RevokeSessionCommand>
{
    private readonly ISessionRepository _repository;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    public RevokeSessionHandler(ISessionRepository repository, IFrankIdentityUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(RevokeSessionCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = SessionTokenHash.From(command.TokenHash);

        await _repository.RevokeAsync(tokenHash, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
