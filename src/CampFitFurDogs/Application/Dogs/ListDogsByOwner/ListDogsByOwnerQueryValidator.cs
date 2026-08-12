using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using FluentValidation;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

/// <summary>
/// Provides structural validation for the <see cref="ListDogsByOwnerQuery"/>.
/// <para>
/// This validator belongs to the Dogs vertical slice and ensures that the caller
/// is requesting only their own dogs. It performs structural validation and
/// identity‑consistency checks by comparing the query's <c>OwnerId</c> with the
/// current authenticated user's identifier.
/// </para>
/// <para>
/// Domain‑level validation (such as verifying that the owner exists or that dogs
/// are present) is handled by the <see cref="IListDogsByOwnerReader"/> implementation.
/// </para>
/// </summary>
public sealed class ListDogsByOwnerQueryValidator : AbstractValidator<ListDogsByOwnerQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListDogsByOwnerQueryValidator"/> class.
    /// <para>
    /// Defines validation rules ensuring that the caller is authorized to list
    /// the requested dogs and that the required identifiers are present. Because
    /// the API endpoint constructs the query using the authenticated user's
    /// identity, this validator ensures structural identity consistency.
    /// </para>
    /// </summary>
    /// <param name="currentUser">
    /// The current authenticated user, used to enforce identity‑consistency constraints.
    /// </param>
    public ListDogsByOwnerQueryValidator(ICurrentUser currentUser)
    {
        /// <summary>
        /// Ensures the owner identifier is provided.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        /// <summary>
        /// Ensures the caller is requesting dogs they actually own.
        /// </summary>
        RuleFor(x => x.OwnerId)
            .Equal(currentUser.Id!.Value);
    }
}
