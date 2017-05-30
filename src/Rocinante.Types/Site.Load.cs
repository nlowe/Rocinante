using System;
using System.IO;
using Jil;

namespace Rocinante.Types
{
    public partial class Site
    {
        public static Site LoadFrom(string path)
        {
            if(!path.EndsWith("site.json")) path += "site.json";
            if(!File.Exists(path))
            {
                throw new NoSuchSiteException(path);
            }

            var site = JSON.Deserialize<Site>(File.ReadAllText(path));
            site.Location = Path.GetDirectoryName(path);

            return site;
        }
    }
}