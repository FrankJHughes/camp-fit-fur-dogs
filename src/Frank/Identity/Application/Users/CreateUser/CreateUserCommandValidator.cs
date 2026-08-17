using FluentValidation;
using Frank.Identity.Application.Abstractions.Users.CreateUser;

namespace Frank.Identity.Application.Users.CreateUser;

/// <summary>
/// Provides semantic validation rules for <see cref="CreateUserCommand"/> that
/// relate specifically to the *identity source* of the user being created.
/// <para>
/// This validator intentionally restricts itself to validating the external
/// identity provider identifier. It does <b>not</b> validate syntactic fields
/// such as first name, last name, email, or phone number.
/// </para>
/// <para>
/// Syntactic validation is handled by the request‑level validator, and domain
/// invariants are enforced by the value objects in <c>Frank.Identity.Domain.Users</c>.
/// </para>
/// </summary>
public sealed class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserCommandValidator"/>
    /// and defines semantic validation rules for the external identity source.
    /// </summary>
    public CreateUserCommandValidator()
    {
        // ─────────────────────────────────────────────────────────────
        // Identity Source Rules (Semantic Validation)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// External identity is required. This ensures that every created user
        /// is associated with an external identity provider (OIDC, SSO, etc.).
        /// </summary>
        RuleFor(x => x.ExternalId)
            .NotEmpty()
            .WithMessage("External provider ID is required.");

        /// <summary>
        /// External identity must be in the format "provider|id". This ensures
        /// that the identity source is unambiguous and can be parsed reliably.
        /// </summary>
        RuleFor(x => x.ExternalId)
            .Must(id => id.Contains('|'))
            .WithMessage("External provider ID must be in the format 'provider|id'.");

        // ─────────────────────────────────────────────────────────────
        // NOTE:
        // We do NOT validate FirstName, LastName, Email, Phone here.
        // Those are syntactic rules handled by the REQUEST VALIDATOR.
        // The DOMAIN enforces invariants via Value Objects.
        // ─────────────────────────────────────────────────────────────
    }
}
