using FluentValidation;

namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Provides FluentValidation rules for validating an incoming
/// <see cref="RegisterDogEndpointRequest"/> payload.
/// <para>
/// This validator enforces syntactic correctness only—required fields,
/// maximum lengths, and ISO‑8601 formatting. It does not apply domain
/// rules or business logic; those responsibilities belong to the
/// Application and Domain layers.
/// </para>
/// </summary>
public sealed class RegisterDogEndpointRequestValidator : AbstractValidator<RegisterDogEndpointRequest>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="RegisterDogEndpointRequestValidator"/> class and configures
    /// all validation rules for the <see cref="RegisterDogEndpointRequest"/>.
    /// </summary>
    public RegisterDogEndpointRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Breed)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("DateOfBirth must be an ISO‑8601 date (yyyy-MM-dd).");

        RuleFor(x => x.Sex)
            .NotEmpty()
            .Must(v => v is "Male" or "Female")
            .WithMessage("Sex must be either 'Male' or 'Female'.");
    }
}
