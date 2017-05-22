namespace Rocinante.Types
{
    public interface IThemeResolver
    {
        /// <summary>
        /// Determines if this theme resolver can resolve the theme for the specified site
        /// </summary>
        /// <param name="site">The site to check</param>
        /// <returns>True iff this resolver can resolve the theme for the site</returns>
        bool CanResolveFor(Site site);

        /// <summary>
        /// Download the theme for the site
        /// </summary>
        /// <param name="site">The site to resolve the theme for</param>
        /// <param name="themePath">The path to extract the theme to</param>
        void ResolveTheme(Site site, string themePath);
    }
}