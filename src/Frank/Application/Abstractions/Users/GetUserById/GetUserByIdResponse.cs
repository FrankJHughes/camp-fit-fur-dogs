namespace Frank.Application.Abstractions.Users.GetUserById;

public record GetUserByIdResponse
(
    Guid Id,
    string FirstName,
    string LastName
);
