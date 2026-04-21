using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public class AutoRegistrationRuleBuilder
{
    private readonly List<AutoRegistrationRule> _rules = new();

    public AutoRegistrationRuleBuilder Add(string suffix, ServiceLifetime lifetime)
    {
        _rules.Add(new AutoRegistrationRule(suffix, lifetime));
        return this;
    }

    public IEnumerable<AutoRegistrationRule> Build() => _rules;
}
