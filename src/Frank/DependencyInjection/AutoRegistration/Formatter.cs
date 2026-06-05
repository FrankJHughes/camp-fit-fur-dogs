using System.Text;
using Frank.DependencyInjection.AutoRegistration.Shapes;

namespace Frank.DependencyInjection.AutoRegistration;

public sealed class Formatter
{
    public static string Format(IReadOnlyList<Violation> violations)
    {
        return string.Join("\n\n", violations.SelectMany(Format));
    }

    public static IEnumerable<string> Format(Violation violation)
    {
        var minCount = violation.MinRegistrationCount;
        var maxCount = violation.MaxRegistrationCount;
        var count = violation.ActualRegistrationCount;

        var implementedInterfaceDeclaration = GetInterfaceDeclaration(violation.Plan.ImplementedInterface);

        var sb = new StringBuilder();

        sb.AppendLine($"{implementedInterfaceDeclaration}");
        sb.AppendLine($"requires between {minCount} and {maxCount} implementations. It has {count}:");

        if (!violation.Plan.ImplementingClasses.Any())
        {
            yield return sb.ToString();
            yield break;
        }

        sb.AppendJoin("\n",
            violation.Plan.ImplementingClasses.Select(c => $"\t{c.Name}"));
        sb.AppendLine();

        yield return sb.ToString();
    }

    static string GetInterfaceDeclaration(Type type)
    {
        if (!type.IsInterface)
            throw new ArgumentException("Type must be an interface.", nameof(type));

        string name = type.Name;
        if (type.IsGenericType)
        {
            // Remove the `N suffix from generic type names
            name = name[..name.IndexOf('`')];

            var args = type.GetGenericArguments()
                           .Select(t => t.Name);

            return $"{name}<{string.Join(", ", args)}>";
        }
        else
        {
            return name;
        }
    }

}
