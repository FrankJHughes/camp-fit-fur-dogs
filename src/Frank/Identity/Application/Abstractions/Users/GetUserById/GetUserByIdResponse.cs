namespace Frank.Identity.Application.Abstractions.Users.GetUserById;

/// <summary>
/// Represents the application‑layer response returned when resolving a user
/// by their internal unique identifier.
/// <para>
/// This response exposes the minimal set of user information required by
/// upstream application flows, such as authentication, profile display, or
/// ownership‑based authorization checks.
/// </para>
/// <para>
/// The response intentionally avoids exposing sensitive or optional fields.
/// Additional user details should be retrieved through dedicated queries when
/// needed.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique internal identifier of the user.
/// </param>
/// <param name="FirstName">
/// The user's given name.
/// </param>
/// <param name="LastName">
/// The user's family name.
/// </param>
public sealed record GetUserByIdResponse(
    Guid Id,
    string FirstName,
    string LastName
);
