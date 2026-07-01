using Frank.Abstractions.Command;
using Frank.Abstractions.UnitOfWork;

using CampFitFurDogs.Application.Abstractions.Sessions.RevokeSession;
using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Application.Sessions.RevokeSession;

public sealed class RevokeSessionHandler : ICommandHandler<RevokeSessionCommand>
{
    private readonly ISessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeSessionHandler(ISessionRepository repository, IUnitOfWork unitOfWork)
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
