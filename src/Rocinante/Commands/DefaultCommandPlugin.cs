using System;
using System.Linq;
using System.Reflection;
using Rocinante.Types;

namespace Rocinante.Commands
{
    internal class DefaultCommandPlugin : IRocinantePlugin
    {
        public void OnLoad(IPublishContext ctx)
        {
            ctx.UseCommand<HelpCommand>()
               .UseCommand<NewCommand>()
               .UseCommand<RestoreCommand>();                
        }
    }
}