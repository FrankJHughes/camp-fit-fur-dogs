using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public record AutoRegistrationRule(string Suffix, ServiceLifetime Lifetime);
