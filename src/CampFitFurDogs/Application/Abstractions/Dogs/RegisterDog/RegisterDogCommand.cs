using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

/// <summary>
/// Represents a command for registering a new dog under a specific owner.
/// <para>
/// This command is part of the Dogs vertical slice and is handled by the
/// <c>RegisterDogCommandHandler</c> in the application layer. It carries the
/// data required to construct a new <c>Dog</c> aggregate and persist it through
/// the <see cref="IRegisterDogWriter"/>.
/// </para>
/// <para>
/// All validation, authorization, and domain rule enforcement occur within the
/// handler and domain model. The command itself is a simple data carrier.
/// </para>
/// </summary>
/// <param name="OwnerId">
/// The unique identifier of the owner registering the dog.
/// </param>
/// <param name="Name">
/// The name of the dog being registered.
/// </param>
/// <param name="Breed">
/// The breed of the dog.
/// </param>
/// <param name="DateOfBirth">
/// The dog’s date of birth.
/// </param>
/// <param name="Sex">
/// The dog’s sex (e.g., "Male", "Female").
/// </param>
public sealed record RegisterDogCommand(
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand<Guid>;
