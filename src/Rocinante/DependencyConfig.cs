using Autofac;
using Autofac.Features.ResolveAnything;

namespace Rocinante
{
    internal static class DependencyConfig
    {
        public static IContainer ConfigureDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            return builder.Build();
        }
    }
}