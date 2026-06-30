using System.Reflection;

namespace CampFitFurDogs.TestUtilities.Infrastructure;

public static class ReflectionExtensions
{
    public static IEnumerable<MethodInfo> ResolveMethodTokens(this Module module, byte[] il)
    {
        for (int i = 0; i < il.Length - 4; i++)
        {
            // call (0x28) or callvirt (0x6F)
            if (il[i] == 0x28 || il[i] == 0x6F)
            {
                int token = BitConverter.ToInt32(il, i + 1);

                MethodBase? mb = null;
                try
                {
                    mb = module.ResolveMethod(token);
                }
                catch
                {
                    // ignore invalid tokens
                }

                if (mb is MethodInfo mi)
                    yield return mi;
            }
        }
    }
}
