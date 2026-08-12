using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.GetDog;

/// <summary>
/// Provides structural validation for the <see cref="GetDogQuery"/>.
/// <para>
/// This validator belongs to the Dogs vertical slice and ensures that the caller
/// is requesting a dog they actually own. It performs structural validation and
/// identity‑consistency checks by comparing the query's <c>OwnerId</c> with the
/// current authenticated user's identifier.
/// </para>
/// <para>
/// Domain‑level validation (such as verifying that the dog exists) is handled by
/// the <see cref="IGetDogReader"/> implementation.
/// </para>
/// </summary>
public sealed class GetDogQueryValidator : AbstractValidator<GetDogQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDogQueryValidator"/> class.
    /// <para>
    /// Defines validation rules ensuring that the caller is authorized to retrieve
    /// the requested dog and that all required identifiers are present. Because
    /// the API endpoint constructs the query using the authenticated user's
    /// identity, this validator ensures structural identity consistency.
    /// </para>
    /// </summary>
    /// <param name="currentUser">
    /// The current authenticated user, used to enforce identity‑consistency constraints.
    /// </param>
    public GetDogQueryValidator(ICurrentUser currentUser)
    {
        /// <summary>
        /// Ensures the owner identifier is provided.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        /// <summary>
        /// Ensures the caller is requesting a dog they actually own.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .Equal(currentUser.Id!.Value);

        /// <summary>
        /// Ensures the dog identifier is provided.
        /// </summary>
        RuleFor(x => x.DogId)
            .NotEmpty();
    }
}
