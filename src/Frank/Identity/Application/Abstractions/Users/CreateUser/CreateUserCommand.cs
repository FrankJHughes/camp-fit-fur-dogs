using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Identity.Application.Abstractions.Users.CreateUser;

/// <summary>
/// Represents a command used to create a new user within the Identity subsystem.
/// <para>
/// This command carries all required information for provisioning a new user
/// record, including personal details, contact information, and an external
/// identity provider identifier.
/// </para>
/// <para>
/// The command handler is responsible for validating the input, ensuring
/// uniqueness constraints (such as email or external ID), and persisting the new
/// user to durable storage.
/// </para>
/// </summary>
/// <remarks>
/// The command returns a <see cref="Guid"/> representing the newly created user's
/// unique identifier.
/// Infrastructure implementations may enforce additional constraints such as:
/// <list type="bullet">
/// <item><description>Email normalization</description></item>
/// <item><description>External identity provider mapping</description></item>
/// <item><description>Optional phone number validation</description></item>
/// </list>
/// </remarks>
/// <param name="FirstName">
/// The user's given name.
/// Must be a non‑empty string.
/// </param>
/// <param name="LastName">
/// The user's family name.
/// Must be a non‑empty string.
/// </param>
/// <param name="Email">
/// The user's email address.
/// Must be unique within the system.
/// </param>
/// <param name="ExternalId">
/// The identifier assigned by an external identity provider (e.g., OIDC subject).
/// </param>
/// <param name="Phone">
/// Optional phone number associated with the user.
/// May be <c>null</c> if not provided.
/// </param>
public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string ExternalId,
    string? Phone = null
) : ICommand<Guid>;
