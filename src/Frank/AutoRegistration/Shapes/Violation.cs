namespace Frank.AutoRegistration.Shapes;

public sealed record Violation(
    Plan Plan,
    int ActualRegistrationCount,
    int MinRegistrationCount,
    int MaxRegistrationCount
);
