using System.Reflection;
using CampFitFurDogs.Api.Horizontal.Startup.Modules;
using CampFitFurDogs.TestUtilities.Infrastructure;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class CorsGuardrailTests
{
    private static readonly Assembly ApiAssembly =
        typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;

    private const string CanonicalConfigKey = "Frontend:BaseUrl";

    // ---------------------------------------------------------------------
    // 1. ONLY CorsStartupModule may configure CORS
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_only_configure_cors_in_CorsStartupModule()
    {
        var corsStartupModuleFullName = typeof(CorsStartupModule).FullName!;

        var offenders = ApiAssembly
            .GetTypes()
            .Where(t => t.FullName != corsStartupModuleFullName &&
                        t.FullName != "Program")
            .SelectMany(t => t.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.Instance))
            .Where(m =>
                // Attribute-based CORS configuration
                m.GetCustomAttributes().Any(a =>
                    a.GetType().Name is "EnableCorsAttribute" or "DisableCorsAttribute") ||

                // Method calls to AddCors or UseCors (reflection-only heuristic)
                MethodBodyCallsCors(m))
            .Select(m => $"{m.DeclaringType!.FullName}.{m.Name}")
            .ToList();

        offenders.Should().BeEmpty(
            "All CORS configuration must be centralized in CorsStartupModule to prevent drift.");
    }

    private static bool MethodBodyCallsCors(MethodInfo method)
    {
        var body = method.GetMethodBody();
        if (body == null)
            return false;

        var il = body.GetILAsByteArray();
        if (il == null || il.Length == 0)
            return false;

        // Reflection-only heuristic:
        // Look for metadata tokens referencing AddCors or UseCors
        return method.Module.ResolveMethodTokens(il)
            .Any(mi =>
                mi.Name is "AddCors" or "UseCors");
    }

    // ---------------------------------------------------------------------
    // 2. NO wildcard CORS APIs
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_not_use_wildcard_cors_apis()
    {
        var offenders = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.Instance))
            .Where(m =>
                m.Name is "AllowAnyOrigin" or "AllowAnyHeader" or "AllowAnyMethod")
            .Select(m => $"{m.DeclaringType!.FullName}.{m.Name}")
            .ToList();

        offenders.Should().BeEmpty(
            "Wildcard CORS APIs must never be used.");
    }

    // ---------------------------------------------------------------------
    // 3. NO wildcard values in CORS configuration
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_not_define_wildcard_values()
    {
        var stringConstants = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static))
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Where(s => s is not null)
            .Cast<string>()
            .ToList();

        stringConstants.Should().NotContain("*",
            "Wildcard origins, headers, or methods must never appear in code.");
    }

    // ---------------------------------------------------------------------
    // 4. NO SetIsOriginAllowed(_ => true)
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_not_use_SetIsOriginAllowed_allow_all()
    {
        var offenders = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.Instance))
            .Where(m => m.Name == "SetIsOriginAllowed")
            .Select(m => $"{m.DeclaringType!.FullName}.{m.Name}")
            .ToList();

        offenders.Should().BeEmpty(
            "SetIsOriginAllowed(_ => true) must never be used.");
    }

    // ---------------------------------------------------------------------
    // 5. NO exposed headers
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_not_expose_headers()
    {
        var offenders = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.Instance))
            .Where(m => m.Name == "WithExposedHeaders")
            .Select(m => $"{m.DeclaringType!.FullName}.{m.Name}")
            .ToList();

        offenders.Should().BeEmpty(
            "Exposed headers must not be used unless explicitly approved.");
    }

    // ---------------------------------------------------------------------
    // 6. ONLY the canonical config key may be used
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_only_use_canonical_frontend_config_key()
    {
        var strings = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static))
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Where(s => s is not null)
            .Cast<string>()
            .Where(s =>
                !s.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) &&
                !s.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                !s.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var configKeyUsages = strings
            .Where(s => s.Contains("Frontend", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        configKeyUsages.Should().OnlyContain(s => s == CanonicalConfigKey,
            "Only the canonical Frontend:BaseUrl key may be used.");
    }

    // ---------------------------------------------------------------------
    // 7. NO multiple frontend config keys
    // ---------------------------------------------------------------------

    [Fact]
    public void Api_should_not_define_multiple_frontend_config_keys()
    {
        var strings = ApiAssembly
            .GetTypes()
            .SelectMany(t => t.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static))
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Where(s => s is not null)
            .Cast<string>()
            .Where(s =>
                !s.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) &&
                !s.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                !s.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var frontendKeys = strings
            .Where(s => s.Contains("Frontend", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        frontendKeys.Should().ContainSingle(
            "Only one canonical frontend config key should exist.");
    }
}
