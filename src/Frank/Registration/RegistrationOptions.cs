namespace Frank.Registration;

public sealed class RegistrationOptions
{
    internal List<Type> ExcludedTypes { get; } = [];
    internal List<Func<Type, bool>> ExclusionPredicates { get; } = [];

    public RegistrationOptions Exclude(params Type[] types)
    {
        ExcludedTypes.AddRange(types);
        return this;
    }

    public RegistrationOptions Exclude(Func<Type, bool> predicate)
    {
        ExclusionPredicates.Add(predicate);
        return this;
    }
}
