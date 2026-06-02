namespace CampFitFurDogs.Application.Authentication.Pipeline;

public static class ReadOnlyListExtensions
{
    public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
                return i;
        }
        return -1;
    }
}
