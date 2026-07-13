namespace Frank.Core.Application.Registration.Shapes;

public sealed record Violation(
    Plan Plan,
    int ActualRegistrationCount,
    int MinRegistrationCount,
    int MaxRegistrationCount
);
