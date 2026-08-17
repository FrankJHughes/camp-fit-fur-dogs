using System.Text;
using Frank.Core.Application.Registration.Shapes;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Provides formatting utilities for producing human‑readable representations
/// of registration <see cref="Violation"/> instances.
///
/// <para>
/// The registration system evaluates discovered interfaces and their
/// implementations against a <see cref="Plan"/>. When the number of
/// implementations falls outside the allowed range, a <see cref="Violation"/>
/// is produced. This formatter converts those violations into structured,
/// readable text suitable for diagnostics, logging, or developer feedback.
/// </para>
/// </summary>
public sealed class Formatter
{
    /// <summary>
    /// Formats a collection of <see cref="Violation"/> instances into a single
    /// multi‑line string. Each violation may expand into multiple formatted
    /// lines depending on the number of implementing classes involved.
    /// </summary>
    /// <param name="violations">
    /// The collection of violations to format.
    /// </param>
    /// <returns>
    /// A multi‑line string containing formatted representations of all violations.
    /// </returns>
    public static string Format(IReadOnlyList<Violation> violations)
    {
        return string.Join("\n\n", violations.SelectMany(Format));
    }

    /// <summary>
    /// Formats a single <see cref="Violation"/> into one or more human‑readable
    /// lines.
    ///
    /// <para>
    /// The output includes:
    /// </para>
    /// <list type="bullet">
    ///   <item>The interface declaration (including generic arguments)</item>
    ///   <item>The required registration range</item>
    ///   <item>The actual number of implementations found</item>
    ///   <item>The list of implementing classes, if any</item>
    /// </list>
    /// </summary>
    /// <param name="violation">The violation to format.</param>
    /// <returns>
    /// A sequence of formatted lines describing the violation.
    /// </returns>
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

    /// <summary>
    /// Produces a readable interface declaration for display, including generic
    /// type arguments when applicable.
    ///
    /// <para>
    /// Generic interface names in .NET include a backtick suffix (e.g.,
    /// <c>IMyInterface`2</c>). This method removes that suffix and formats the
    /// generic arguments into a conventional C# type declaration such as:
    /// <c>IMyInterface&lt;T1, T2&gt;</c>.
    /// </para>
    /// </summary>
    /// <param name="type">The interface type to format.</param>
    /// <returns>
    /// A readable interface declaration string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided type is not an interface.
    /// </exception>
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
