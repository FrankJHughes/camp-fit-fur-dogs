using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.RemoveDog;

/// <summary>
/// Provides structural validation for the <see cref="RemoveDogCommand"/>.
/// <para>
/// This validator belongs to the Dogs vertical slice and ensures that the caller
/// is authorized to remove the specified dog. It performs structural validation
/// only, verifying that required identifiers are present and that the caller is
/// the owner associated with the command.
/// </para>
/// <para>
/// Existence checks and domain‑level validation are handled by the
/// <see cref="RemoveDogCommandHandler"/> and the underlying domain model.
/// </para>
/// </summary>
public sealed class RemoveDogCommandValidator : AbstractValidator<RemoveDogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveDogCommandValidator"/> class.
    /// <para>
    /// Defines validation rules ensuring that the caller is authorized to remove
    /// the dog and that the command contains all required identifiers.
    /// </para>
    /// </summary>
    /// <param name="currentUser">
    /// The current authenticated user, used to enforce ownership constraints.
    /// </param>
    public RemoveDogCommandValidator(ICurrentUser currentUser)
    {
        /// <summary>
        /// Ensures the dog identifier is provided.
        /// </summary>
        RuleFor(x => x.DogId)
            .NotEmpty();

        /// <summary>
        /// Ensures the owner identifier is provided.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        /// <summary>
        /// Ensures the caller is attempting to remove a dog they own.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .Equal(currentUser.Id!.Value);
    }
}
