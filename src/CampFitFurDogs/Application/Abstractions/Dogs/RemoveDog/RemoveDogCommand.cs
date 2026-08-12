using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;

/// <summary>
/// Represents a command for removing an existing dog owned by a specific user.
/// <para>
/// This command is part of the Dogs vertical slice and is handled by the
/// <c>RemoveDogCommandHandler</c> in the application layer. It carries the
/// identifiers required to locate the dog aggregate and authorize the removal
/// operation.
/// </para>
/// <para>
/// All validation, authorization, and domain rule enforcement occur within the
/// handler and domain model. The command itself is a simple data carrier.
/// </para>
/// </summary>
/// <param name="DogId">
/// The unique identifier of the dog being removed.
/// </param>
/// <param name="OwnerId">
/// The unique identifier of the owner performing the removal. Used to enforce
/// ownership and authorization rules.
/// </param>
public sealed record RemoveDogCommand(
    Guid DogId,
    Guid OwnerId) : ICommand;
