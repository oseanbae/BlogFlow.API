using System.Text.RegularExpressions;

namespace BlogFlow.API.Helper
{
    public static class SlugHelper
    {
        public static string Normalize(string name)
        {
            var collapsed = Regex.Replace(name.Trim(), @"\s+", " ");
            return collapsed.ToLowerInvariant().Replace(" ", "-");
        }
    }
}
