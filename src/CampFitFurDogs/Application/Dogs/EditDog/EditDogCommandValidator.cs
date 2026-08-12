using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Domain.Dogs;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.EditDog;

/// <summary>
/// Provides structural validation for the <see cref="EditDogCommand"/>.
/// <para>
/// This validator belongs to the Dogs vertical slice and ensures that all
/// incoming edit requests contain valid, well‑formed data before reaching the
/// <c>EditDogCommandHandler</c>. It performs structural validation only; domain
/// invariants are enforced by the <see cref="Dog"/> aggregate.
/// </para>
/// <para>
/// Because the API endpoint constructs the command using the authenticated
/// user's identity, this validator also ensures structural identity consistency
/// by verifying that the <c>OwnerId</c> matches the current user.
/// </para>
/// </summary>
public sealed class EditDogCommandValidator : AbstractValidator<EditDogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditDogCommandValidator"/> class.
    /// <para>
    /// Defines validation rules for dog editing, including required identifiers,
    /// non‑empty fields, valid enum mapping, and identity‑consistency checks.
    /// </para>
    /// </summary>
    /// <param name="currentUser">
    /// The current authenticated user, used to enforce identity‑consistency constraints.
    /// </param>
    public EditDogCommandValidator(ICurrentUser currentUser)
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
        /// Ensures the owner identifier matches the authenticated user.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .Equal(currentUser.Id!.Value);

        /// <summary>
        /// Ensures the dog name is provided.
        /// </summary>
        RuleFor(x => x.Name)
            .NotEmpty();

        /// <summary>
        /// Ensures the dog breed is provided.
        /// </summary>
        RuleFor(x => x.Breed)
            .NotEmpty();

        /// <summary>
        /// Ensures the sex value is valid and maps to the <see cref="Sex"/> enum.
        /// </summary>
        RuleFor(x => x.Sex)
            .Must(s => Enum.TryParse<Sex>(s, ignoreCase: true, out _))
            .WithMessage("Sex must be 'Male' or 'Female'.");
    }
}
