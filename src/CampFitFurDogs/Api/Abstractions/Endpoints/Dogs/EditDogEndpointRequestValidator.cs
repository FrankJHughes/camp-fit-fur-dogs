using FluentValidation;

namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Provides FluentValidation rules for validating an incoming
/// <see cref="EditDogEndpointRequest"/> payload.
/// <para>
/// This validator enforces syntactic correctness only—required fields,
/// maximum lengths, and ISO‑8601 formatting. It does not apply domain
/// rules or business logic; those are handled in the Application layer.
/// </para>
/// </summary>
public sealed class EditDogEndpointRequestValidator : AbstractValidator<EditDogEndpointRequest>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="EditDogEndpointRequestValidator"/> class and configures
    /// all validation rules for the <see cref="EditDogEndpointRequest"/>.
    /// </summary>
    public EditDogEndpointRequestValidator()
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
