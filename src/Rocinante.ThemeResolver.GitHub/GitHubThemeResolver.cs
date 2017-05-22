using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using NLog;
using Rocinante.Types;
using Rocinante.Types.Extensions;

namespace Rocinante.ThemeResolver.GitHub
{
    public class GitHubThemeResolver : IThemeResolver, IRocinantePlugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private const string GITHUB_API_ENDPOINT = "https://api.github.com";
        private const string GITHUB_THEME_PATTERN = @"^([-_\w]+)\/([-_.\w]+)(?:#|@)?([-_.\w]+)?$";
        private readonly Regex Matcher = new Regex(GITHUB_THEME_PATTERN, RegexOptions.Compiled);

        public bool CanResolveFor(Site site) => Matcher.IsMatch(site.Theme);

        public void ResolveTheme(Site site, string themePath)
        {
            var parts = Matcher.Match(site.Theme);

            if(!parts.Success || parts.Captures.Count < 2 || parts.Captures.Count > 3)
            {
                throw new Exception($"The github theme resolver does not know how to resolve {site.Theme}. This should never happen...");
            }

            var group = parts.Captures[0].Value;
            var repo = parts.Captures[1].Value;

            var branchOrTag = parts.Captures.Count == 3 ? parts.Captures[2].Value : null;

            Log.Debug("Downloading {0}/{1}", group, repo);
            if(!branchOrTag.IsNullOrEmpty())
            {
                Log.Trace("Downloading branch or tag {0}", branchOrTag);
            }
            else
            {
                Log.Trace("Defaulting to branch master");
                branchOrTag = "master";
            }

            var uri = $"{GITHUB_API_ENDPOINT.AppendIfMissing("/")}repos/{group}/{repo}/zipball/{branchOrTag}";
            
            Log.Trace("Downloading repo zip {0}", uri);
            using(var client = new HttpClient())
            using(var zipStream = new ZipArchive(client.GetStreamAsync(uri).Result))
            {
                zipStream.ExtractToDirectory(themePath);
            }
        }

        public void OnLoad(IPublishContext ctx)
        {
            ctx.UseThemeResolver<GitHubThemeResolver>();
        }
    }
}
