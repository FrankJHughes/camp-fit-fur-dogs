using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Abstractions.Dogs.EditDog;

/// <summary>
/// Represents a request to update an existing dog owned by a specific user.
/// <para>
/// This command is part of the Dogs vertical slice and is handled by the
/// corresponding <c>EditDogCommandHandler</c> in the application layer.
/// </para>
/// <para>
/// The command carries only the data required to perform the update operation.
/// Validation, authorization, and business rules are enforced by the handler
/// and domain model.
/// </para>
/// </summary>
/// <param name="DogId">
/// The unique identifier of the dog being edited.
/// </param>
/// <param name="OwnerId">
/// The unique identifier of the owner performing the edit. Used for
/// authorization and ownership validation.
/// </param>
/// <param name="Name">
/// The updated name of the dog.
/// </param>
/// <param name="Breed">
/// The updated breed of the dog.
/// </param>
/// <param name="DateOfBirth">
/// The updated date of birth of the dog.
/// </param>
/// <param name="Sex">
/// The updated sex of the dog (e.g., "Male", "Female").
/// </param>
public sealed record EditDogCommand(
    Guid DogId,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand;
