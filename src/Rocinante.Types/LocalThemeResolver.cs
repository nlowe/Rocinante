using System;
using System.IO;

namespace Rocinante.Types
{
    /// <summary>
    /// A Theme resovler that simply ensures the theme directory exists. This lets developers work on new themes locally
    /// </summary>
    public class LocalThemeResolver : IThemeResolver
    {
        public const string IDENTIFIER = "_local_";

        public bool CanResolveFor(Site site) => site.Theme.Equals(IDENTIFIER, StringComparison.OrdinalIgnoreCase);

        public void ResolveTheme(Site site, string themePath)
        {
            if(!Directory.Exists(themePath))
            {
                Directory.CreateDirectory(themePath);
            }
        }
    }
}