using System;

namespace Rocinante.Types
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Help { get; }

        void Execute(string[] args, IPublishContext ctx);
    }
}