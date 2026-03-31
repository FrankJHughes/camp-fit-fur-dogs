namespace CampFitFurDogs.SharedKernel;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    // RED: no Equals/GetHashCode override — defaults to reference equality
}