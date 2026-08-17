namespace Frank.Core.Domain;

/// <summary>
/// Base class for all value objects in the domain.
///
/// Value objects are immutable and define equality based on their
/// structural components rather than identity. To implement a value
/// object, override <see cref="GetEqualityComponents"/> and return
/// the set of fields that participate in equality.
///
/// This class provides:
/// - Structural equality
/// - Consistent hash code generation
/// - Type-based equality enforcement
/// - Operator overloads for == and !=
///
/// Value objects must be immutable and should never expose setters.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Returns the components that define equality for this value object.
    /// Derived classes must return all fields that participate in equality.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// Equality is structural and type-specific.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Equality operator for value objects.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
        => Equals(left, right);

    /// <summary>
    /// Inequality operator for value objects.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !Equals(left, right);

    /// <summary>
    /// Computes a hash code based on the equality components.
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, component) =>
            {
                unchecked
                {
                    return current * 23 + (component?.GetHashCode() ?? 0);
                }
            });
    }
}

/// <summary>
/// A generic value object base class for simple single-value value objects.
///
/// This class reduces boilerplate for common value objects such as:
/// - Email
/// - PhoneNumber
/// - FirstName
/// - LastName
/// - ExternalId
/// - SessionId
/// - AggregateId
///
/// It provides:
/// - A strongly typed <see cref="Value"/> property
/// - Structural equality based on the underlying value
/// - A consistent <see cref="ToString"/> implementation
///
/// Example:
/// <code>
/// public sealed class Email : ValueObject<string>
/// {
///     public Email(string value) : base(value)
///     {
///         // validation...
///     }
/// }
/// </code>
/// </summary>
/// <typeparam name="T">The underlying primitive type.</typeparam>
public abstract class ValueObject<T> : ValueObject
{
    /// <summary>
    /// The underlying primitive value represented by this value object.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObject{T}"/> class.
    /// </summary>
    protected ValueObject(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Returns the underlying value as the sole equality component.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns a string representation of the underlying value.
    /// </summary>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}
