using System.Linq;

namespace Tc.Usd.SingleSignOnLogin
{
    public static class StringSearchHelper
    {
        public static bool ContainsAll(this string source, params string[] values)
        {
            return values.All(source.Contains);
        }
    }
}