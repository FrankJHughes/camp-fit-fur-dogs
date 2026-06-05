using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using FluentValidation;

public sealed class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // ─────────────────────────────────────────────────────────────
        // Identity Source Rules (Semantic Validation)
        // ─────────────────────────────────────────────────────────────

        // Must have AT LEAST ONE identity source
        RuleFor(x => x)
            .Must(cmd =>
                !(string.IsNullOrWhiteSpace(cmd.Password) &&
                  string.IsNullOrWhiteSpace(cmd.ExternalAuthProviderId)))
            .WithMessage("Customer must have either a password or an external provider ID.");

        // Must NOT have BOTH identity sources
        RuleFor(x => x)
            .Must(cmd =>
                !(!string.IsNullOrWhiteSpace(cmd.Password) &&
                  !string.IsNullOrWhiteSpace(cmd.ExternalAuthProviderId)))
            .WithMessage("Customer cannot have both a password hash and an external provider ID.");

        // ─────────────────────────────────────────────────────────────
        // Password Hash (Semantic Validation)
        // ─────────────────────────────────────────────────────────────

        When(x => x.Password is not null, () =>
        {
            RuleFor(x => x.Password!)
                .Must(hash =>
                    hash.StartsWith("$2a$") ||
                    hash.StartsWith("$2b$") ||
                    hash.StartsWith("$2y$"))
                .WithMessage("Password must already be hashed using BCrypt.");
        });

        // ─────────────────────────────────────────────────────────────
        // External Provider ID (Semantic Validation)
        // ─────────────────────────────────────────────────────────────

        When(x => x.ExternalAuthProviderId is not null, () =>
        {
            RuleFor(x => x.ExternalAuthProviderId!)
                .Must(id => id.Contains('|'))
                .WithMessage("External provider ID must be in the format 'provider|id'.");
        });

        // ─────────────────────────────────────────────────────────────
        // NOTE:
        // We do NOT validate FirstName, LastName, Email, Phone here.
        // Those are syntactic rules handled by the REQUEST VALIDATOR.
        // The DOMAIN enforces invariants via Value Objects.
        // ─────────────────────────────────────────────────────────────
    }
}
