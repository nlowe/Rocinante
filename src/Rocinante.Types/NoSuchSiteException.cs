using System;

namespace Rocinante.Types
{
    public class NoSuchSiteException : Exception
    {
        public NoSuchSiteException(string path) : base($"Could not locate a rocinante site at {path}")
        {
        }
    }
}