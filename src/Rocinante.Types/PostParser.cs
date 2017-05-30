using System.IO;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Rocinante.Types
{
    public static class PostParser
    {
        private static readonly Regex FrontmatterRegex = new Regex(@"(^---$[\r\n]+(?:.*[\r\n])+)^---$", RegexOptions.Multiline | RegexOptions.Compiled);

        public static bool TryLoad(string path, out Post post)
        {
            if(!File.Exists(path))
            {
                post = null;
                return false;
            }

            var raw = File.ReadAllText(path).TrimStart();
            var match = FrontmatterRegex.Match(raw);
            if(!match.Success)
            {
                post = null;
                return false;
            }

            var frontMatter = match.Groups[1].Value;
            var remaining = raw.Substring(match.Index + match.Length);

            var parser = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            
            post = parser.Deserialize<Post>(frontMatter);
            post.Content = remaining;

            return true;
        }
    }
}