namespace CampFitFurDogs.SharedKernel;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; }

    protected Entity(TId id) => Id = id;

    // RED: no Equals/GetHashCode override — defaults to reference equality
}