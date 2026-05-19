namespace CampFitFurDogs.TestUtilities.Builders;

public abstract class TestDataBuilderBase<TBuilder, TObject>
    where TBuilder : TestDataBuilderBase<TBuilder, TObject>, new()
{
    protected TBuilder Clone() => (TBuilder)MemberwiseClone();

    public TBuilder With(System.Action<TBuilder> mutate)
    {
        var clone = Clone();
        mutate(clone);
        return clone;
    }

    public abstract TObject Build();
}
