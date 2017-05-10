using System;

namespace Rocinante.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Help { get; }

        void Execute(string[] args);
    }
}