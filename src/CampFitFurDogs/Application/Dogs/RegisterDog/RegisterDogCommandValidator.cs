using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

/// <summary>
/// Provides structural validation for the <see cref="RegisterDogCommand"/>.
/// <para>
/// This validator belongs to the Dogs vertical slice and ensures that all
/// required fields for registering a new dog are present and well‑formed before
/// reaching the <c>RegisterDogCommandHandler</c>. It performs structural validation
/// only; domain invariants are enforced by the <c>Dog</c> aggregate and its
/// supporting value objects.
/// </para>
/// <para>
/// Because the API endpoint constructs the command using the authenticated
/// user's identity, this validator also ensures that the <c>OwnerId</c>
/// contained in the command matches the current user, guaranteeing structural
/// identity consistency.
/// </para>
/// </summary>
public sealed class RegisterDogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDogCommandValidator"/> class.
    /// <para>
    /// Defines validation rules for dog registration, including required
    /// identifiers, non‑empty fields, a valid birth date, a strict check
    /// ensuring that the provided sex value is either "Male" or "Female",
    /// and a structural identity consistency check ensuring that the
    /// <c>OwnerId</c> matches the authenticated user.
    /// </para>
    /// </summary>
    /// <param name="currentUser">
    /// The current authenticated user, used to enforce identity consistency.
    /// </param>
    public RegisterDogCommandValidator(ICurrentUser currentUser)
    {
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
        /// Ensures the date of birth is in the past.
        /// </summary>
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow));

        /// <summary>
        /// Ensures the sex value is provided and valid.
        /// </summary>
        RuleFor(x => x.Sex)
            .NotEmpty()
            .Must(s =>
                s.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("Female", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sex must be 'Male' or 'Female'.");
    }
}
